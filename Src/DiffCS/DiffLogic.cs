using System.Collections.Generic;
using System.Linq;

using static DiffCS.Definition;

namespace DiffCS
{
    public static class DiffLogic
    {
        /// <summary>
        /// Get Diff of str1 and str2.
        /// </summary>
        /// <param name="OldData">param1</param>
        /// <param name="NewData">param2</param>
        public static List<ResultTreeNode> GetDiff<T>(IEnumerable<T> OldData, IEnumerable<T> NewData)
        {
            List<TreeNode> nodeList;
            Dictionary<int, TreeNode> currentNode;
            
            // Initialize Params
            bool isSwap = OldData.Count() > NewData.Count();

            int[] sHashData = !isSwap ? GetHashList(OldData) : GetHashList(NewData);
            int[] lHashData = !isSwap ? GetHashList(NewData) : GetHashList(OldData);

            int delta = lHashData.Length - sHashData.Length;

            // Create Node List
            nodeList = new List<TreeNode>();
            nodeList.Add(new TreeNode()
            {
                PropK = 0,
                PosY = 0,
                PrevNode = null
            });
            currentNode = new Dictionary<int, TreeNode>(sHashData.Length + lHashData.Length + 3);
            currentNode.Add(0, nodeList[0]);
            
            int p = 0;

            while (!(currentNode.ContainsKey(delta) && currentNode[delta].PosY == sHashData.Length))
            {
                for (int k = -p; k < delta; k++)
                {
                    Snake(k, sHashData, lHashData, currentNode, nodeList);
                }

                for (int k = delta + p; k > delta; k--)
                {
                    Snake(k, sHashData, lHashData, currentNode, nodeList);
                }

                Snake(delta, sHashData, lHashData, currentNode, nodeList);

                p = p + 1;
            }
            p = p - 1;

            // generate result list
            var output = GenerateResultList(nodeList, isSwap, OldData, NewData);

            return output;
        }

        private static int[] GetHashList<T>(IEnumerable<T> Source)
        {
            int[] result = new int[Source.Count()];
            foreach (var item in Source.Select((v, i) => new { v, i }))
            {
                result[item.i] = item.v.GetHashCode();
            }

            return result;
        }

        private static void Snake(int PropK, int[] shortObj, int[] largeObj,
            Dictionary<int, TreeNode> currentList, List<TreeNode> nodeList)
        {
            // out of Matrix Range
            if (PropK < -shortObj.Length || largeObj.Length < PropK)
            {
                return;
            }

            // from -1 Line
            if (currentList.ContainsKey(PropK - 1)
                && (!currentList.ContainsKey(PropK + 1) || currentList[PropK + 1].PosY + 1 < currentList[PropK - 1].PosY + 1))
            {
                TreeNode addNode = new TreeNode();

                addNode.PosY = currentList[PropK - 1].PosY;
                addNode.PropK = PropK;
                addNode.PrevNode = currentList[PropK - 1];

                currentList[PropK] = addNode;
                nodeList.Add(addNode);
            }


            // from +1 Line
            else if (currentList.ContainsKey(PropK + 1)
                && (!currentList.ContainsKey(PropK - 1) || currentList[PropK - 1].PosY < currentList[PropK + 1].PosY + 1))
            {
                TreeNode addNode = new TreeNode();

                addNode.PosY = currentList[PropK + 1].PosY + 1;
                addNode.PropK = PropK;
                addNode.PrevNode = currentList[PropK + 1];

                currentList[PropK] = addNode;
                nodeList.Add(addNode);
            }

            // on Line
            if (currentList.ContainsKey(PropK))
            {
                TreeNode addNode = new TreeNode();

                addNode.PosY = currentList[PropK].PosY;
                addNode.PropK = PropK;
                addNode.PrevNode = currentList[PropK];

                while (true)
                {
                    if (addNode.PosY < shortObj.Length && addNode.PosY + PropK < largeObj.Length
                        && shortObj[addNode.PosY] == largeObj[addNode.PosY + PropK])
                    {
                        addNode.PosY++;
                        continue;
                    }
                    break;
                }

                if (currentList[PropK].PosY < addNode.PosY)
                {
                    currentList[PropK] = addNode;
                    nodeList.Add(addNode);
                }
            }
        }

        private static List<ResultTreeNode> GenerateResultList<T>(List<TreeNode> nodeList, bool isSwap, IEnumerable<T> oldData, IEnumerable<T> newData)
        {
            List<ResultTreeNode> result = new List<ResultTreeNode>();
            var target = nodeList.Last();

            while (target.PosY > 0 || target.PropK != 0)
            {
                if (target.PropK == target.PrevNode.PropK)
                {
                    target = target.PrevNode;
                    continue;
                }
                else
                {
                    ResultTreeNode resultNode = new ResultTreeNode();
                    if (!isSwap)
                    {
                        resultNode.OldDataIndex = target.PosY;
                        resultNode.NewDataIndex = target.PosY + target.PropK;

                        if (target.PropK > target.PrevNode.PropK)
                        {
                            resultNode.DiffType = DIFF_TYPE.DIFF_ADD;
                            resultNode.DiffObjcet = newData.ToArray()[resultNode.NewDataIndex - 1];
                        }
                        else
                        {
                            resultNode.DiffType = DIFF_TYPE.DIFF_DEL;
                            resultNode.DiffObjcet = oldData.ToArray()[resultNode.OldDataIndex - 1];
                        }
                    }
                    else
                    {
                        resultNode.OldDataIndex = target.PosY + target.PropK;
                        resultNode.NewDataIndex = target.PosY;

                        if (target.PropK > target.PrevNode.PropK)
                        {
                            resultNode.DiffType = DIFF_TYPE.DIFF_DEL;
                            resultNode.DiffObjcet = oldData.ToArray()[resultNode.OldDataIndex - 1];
                        }
                        else
                        {
                            resultNode.DiffType = DIFF_TYPE.DIFF_ADD;
                            resultNode.DiffObjcet = newData.ToArray()[resultNode.NewDataIndex - 1];
                        }
                    }

                    result.Insert(0, resultNode);
                }
                target = target.PrevNode;
            }

            return result;
        }
    }
}
