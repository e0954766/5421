using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ConsoleApp1
{
    public class Generator
    {
        private readonly GameConfig gameConfig;

        public Generator(GameConfig gameConfig)
        {
            this.gameConfig = gameConfig;
        }

        public JObject Generate()
        {
            int numAttributes = (int)gameConfig.GetValue("attributes");
            var attributeSet =
                new HashSet<string>(Enumerable.Range('A', numAttributes).Select(x => ((char)x).ToString()));

            int numFds = (int)gameConfig.GetValue("fds");
            var fixedCountParams = gameConfig.GetFixedCountLhsRhs();
            int fixedLhsCount = fixedCountParams["fixed_lhs_count"];
            int fixedLhsSize = fixedCountParams["fixed_lhs_size"];
            int fixedRhsCount = fixedCountParams["fixed_rhs_count"];
            int fixedRhsSize = fixedCountParams["fixed_rhs_size"];

            var lhsSizeGenerator = gameConfig.GetLhsSizeGenerator();
            var rhsSizeGenerator = gameConfig.GetRhsSizeGenerator();

            var fds = new List<List<string>>();
            var allLhs = new HashSet<string>();
            int fixedLhsGenerated = 0;
            int fixedRhsGenerated = 0;

            while (fds.Count < numFds)
            {
                int lhsSize;
                if (fixedLhsGenerated < fixedLhsCount)
                {
                    lhsSize = fixedLhsSize;
                    fixedLhsGenerated++;
                }
                else
                {
                    lhsSize = (int)lhsSizeGenerator.Generate(gameConfig.GetConfig()["lhs_size"].ToObject<JObject>());
                    while (lhsSize >= attributeSet.Count && lhsSize < 1)
                    {
                        lhsSize = (int)lhsSizeGenerator.Generate(gameConfig.GetConfig()["lhs_size"]
                            .ToObject<JObject>());
                    }
                }

                var lhs = attributeSet.OrderBy(x => Guid.NewGuid()).Take(lhsSize).ToList();
                lhs.Sort();
                var lhsStr = string.Join("", lhs);
                

                if (allLhs.Contains(lhsStr))
                {
                    if (fixedLhsGenerated - 1 < fixedLhsCount && fixedLhsCount > 0)
                    {
                        fixedLhsGenerated--;
                    }

                    continue;
                }
                else
                {
                    allLhs.Add(lhsStr);
                }

                int rhsSize;
                if (fixedRhsGenerated < fixedRhsCount)
                {
                    if (new[] { true, false }.OrderBy(x => Guid.NewGuid()).First())
                    {
                        rhsSize = fixedRhsSize;
                        fixedRhsGenerated++;
                    }
                    else
                    {
                        rhsSize = (int)rhsSizeGenerator.Generate(gameConfig.GetConfig()["rhs_size"]
                            .ToObject<JObject>());
                    }
                }
                else
                {
                    rhsSize = (int)rhsSizeGenerator.Generate(gameConfig.GetConfig()["rhs_size"].ToObject<JObject>());
                }

                if (rhsSize == 0)
                {
                    rhsSize = 1;
                }

                var rhs = attributeSet.Except(lhs).OrderBy(x => Guid.NewGuid())
                    .Take(Math.Min(rhsSize, attributeSet.Count - lhs.Count - 1)).ToList();
                rhs.Sort();

                fds.Add(new List<string>(new[] { lhsStr, string.Join("", rhs) }));
            }

            var candidateKeys = FindCandidateKeys(attributeSet, fds);
            var sortedAttributeSet = attributeSet.OrderBy(x => x);
            JObject data = new JObject
            {
                { "R", new JArray(sortedAttributeSet) },
                { "FD", new JArray(fds.Select(fd => new JArray(new JArray(fd[0]), new JArray(fd[1])))) },
                { "CK", new JArray(candidateKeys.Select(ck => new JArray(ck))) }
            };


            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "data.json");
            File.WriteAllText(filePath, json);

            return data;
        }

        private List<List<string>> FindCandidateKeys(HashSet<string> attributeSet, List<List<string>> fds)
        {
            return AllClosures(attributeSet, fds);
        }

        private HashSet<string> Closure(List<List<string>> fds, HashSet<string> s)
        {
            var closureResult = new HashSet<string>(s);
            bool lastChanged = true;
            while (lastChanged)
            {
                lastChanged = false;
                foreach (var fd in fds)
                {
                    string str0 = fd[0];
                    char[] chars0 = str0.ToCharArray();
                    string[] strings0 = Array.ConvertAll(chars0, x => x.ToString());
                    string str1 = fd[1];
                    char[] chars1 = str1.ToCharArray();
                    string[] strings1 = Array.ConvertAll(chars1, x => x.ToString());
                    var leftSide = new HashSet<string>(strings0);
                    var rightSide = new HashSet<string>(strings1);
                    if (leftSide.IsSubsetOf(closureResult))
                    {
                        if (!rightSide.IsSubsetOf(closureResult))
                        {
                            foreach (var attrs in rightSide)
                            {
                                closureResult.Add(attrs);
                                lastChanged = true;
                            }
                        }
                    }
                }
            }

            return new HashSet<string>(closureResult.OrderBy(x => x));
        }

        private List<List<string>> AllClosures(HashSet<string> R, List<List<string>> F)
        {
            var attributeClosures = new List<List<string>>();
            var candidates = new List<HashSet<string>>();

            for (int i = 1; i <= R.Count; i++)
            {
                foreach (var attrs in GetCombinations(R, i))
                {
                    var curClosure = Closure(F, new HashSet<string>(attrs));
                    var isSuperkey = curClosure.Count == R.Count;
                    if (isSuperkey)
                    {
                        var add = true;
                        foreach (var key in candidates)
                        {
                            if (key.IsSubsetOf(new HashSet<string>(attrs)))
                            {
                                add = false;
                                break;
                            }
                        }

                        if (add)
                        {
                            candidates.Add(new HashSet<string>(attrs));
                        }
                    }
                }
            }

            return candidates.Select(x => x.OrderBy(y => y).ToList()).ToList();
        }

        private IEnumerable<IEnumerable<T>> GetCombinations<T>(IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new[] { t });
            return GetCombinations(list, length - 1)
                .SelectMany(t => list.Where(e => !t.Contains(e)), (t1, t2) => t1.Concat(new[] { t2 }));
        }
    }
}