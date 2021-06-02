using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AGRO_GRAMM
{
    class Cube
    {
        private Dictionary<int, Dictionary<int, Dictionary<string, int>>> cube;

        private static Cube privateCube;

        // int = 1, float = 2, char = 3
        public static string json = @"{
				""1"": {
					""1"": {
						""+"": 1,
						""-"": 1,
						""/"": 1,
						""*"": 1,
						""**"": 1,
						""//"": 1,
						""%"": 1,
						""+="": 1,
						""-="": 1,
						""/="": 1,
						""*="": 1,
						""++"": 1,
						""--"": 1,
						""<"": 1,
						""<="": 1,
						"">"": 1,
						"">="": 1,
						""=="": 1,
						""="": 1,
						""!="": 1,
						""&&"": 1,
						""and"": 1,
						""or"": 1,
						""||"": 1,
						""print"": 1,
						""input"": 1
					},
					""2"": {
						""+"": 2,
						""-"": 2,
						""/"": 2,
						""*"": 2,
						""//"": 1,
						""**"": 2,
						""%"": 1,
						""<"": 1,
						""<="": 1,
						"">"": 1,
						"">="": 1,
						""=="": 1,
						""="": 1,
						""+="": 1,
						""-="": 1,
						""/="": 1,
						""*="": 1,
						""!="": 1
					},
					""3"": {
						""+"": 1,
						""-"": 1,
						""="": 1,
						""+="": 1,
						""-="": 1
					},
					""6"":{
						""+"": 6,
						""*"": 6
					}

				},
				""2"": {
					""1"": {
						""+"": 2,
						""-"": 2,
						""/"": 2,
						""*"": 2,
						""//"": 1,
						""**"": 2,
						""%"": 1,
						""<"": 1,
						""<="": 1,
						"">"": 1,
						"">="": 1,
						""=="": 1,
						""=="": 1,
						""="": 2,
						""+="": 2,
						""-="": 2,
						""/="": 2,
						""*="": 2,
						""!="": 1
					},
					""2"": {
						""+"": 2,
						""-"": 2,
						""/"": 2,
						""*"": 2,
						""//"": 1,
						""**"": 2,
						""%"": 1,
						""<"": 1,
						""<="": 1,
						"">"": 1,
						"">="": 1,
						""=="": 1,
						""="": 2,
						""+="": 2,
						""-="": 2,
						""/="": 2,
						""*="": 2,
						""++"": 2,
						""--"": 2,
						""!="": 1,
						""print"": 2,
						""input"": 2
					},
					""3"": {
					},
					""6"":{
						""+"": 6
					}
				},
				""3"": {
					""1"": {
						""+"": 3,
						""-"": 3,
						""="": 3,
						""+="": 3,
						""-="": 3
					},
					""2"": {
					},
					""3"": {
						""+"": 3,
						""-"": 3,
						""*"": 3,
						""/"": 3,
						""<"": 1,
						""<="": 1,
						"">"": 1,
						"">="": 1,
						""=="": 1,
						""="": 3,
						""+="": 3,
						""-="": 3,
						""/="": 3,
						""*="": 3,
						""++"": 3,
						""--"": 3,
						""!="": 1,
						""print"": 3,
						""input"": 3
					},
					""6"":{
						""+"": 6,
					}
				},
				""6"": {
					""1"": {
						""+"": 6,
						""*"": 6
					},
					""2"": {
						""+"": 6
					},
					""3"": {
						""+"": 3
					},
					""6"":{
						""+"": 6,
						""print"": 6,
						""input"": 6,
						""=="": 1,
						""!="": 1,
						""="": 6
					}
				},
			}";

        public static Cube getInstance()
        {
            if (privateCube == null)
            {
                privateCube = new Cube();
            }

            return privateCube;
        }

        private Cube()
        {
            cube = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, Dictionary<string, int>>>>(json);
        }

        /// <summary>
        /// Searches in semantic operator cube
        /// </summary>
        /// <param name="typeA">Left-side operand</param>
        /// <param name="typeB">Right-side operand</param>
        /// <param name="op">Operator</param>
        /// <returns>Operator int >= 1. If error returns -1</returns>
        public int outputCube(int typeA, int typeB, int op, Dictionary<int, string> dict)
        {
            if (cube.ContainsKey(typeA))
            {
                if (cube[typeA].ContainsKey(typeB))
                {
                    if (cube[typeA][typeB].ContainsKey(dict[op]))
                    {
                        return cube[typeA][typeB][dict[op]];
                    }
                }
            }

            return Int32.MaxValue;
        }

    }
}
