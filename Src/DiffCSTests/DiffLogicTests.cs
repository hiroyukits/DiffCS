using Microsoft.VisualStudio.TestTools.UnitTesting;
using DiffCS.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffCS.Logic.Tests
{
    [TestClass()]
    public class DiffLogicTests
    {
        [TestMethod()]
        public void StaticTextTest()
        {
            string newText = @"
<!DOCTYPE html>
<html>
    <head>
        <meta charset=""UTF - 8"">
        <title> HTML5 </title>
    </head>
    <body>
        <p> Written on HTML5. </p>
    </body>
</html>
";

            string oldText = @"
<!DOCTYPE HTML PUBLIC "" -//W3C//DTD HTML 4.01 Transitional//EN"" ""http://www.w3.org/TR/html4/loose.dtd"">
<html>
    <head>
        <meta http-equiv=""Content - Type"" content=""text / html; charset = UTF - 8"">
        <title> HTML4 </title>
    </head>
    <body>
        <p> Written on HTML4. </p>
    </body>
</html>
";

            Assert.IsTrue(GetDiffTest(oldText, newText));
        }

        [TestMethod()]
        public void RundomTest()
        {
            var sb = new StringBuilder();

            System.Random r = new System.Random((int)(DateTime.Now.ToBinary() % int.MaxValue));

            // Generate random oldText.
            int len = r.Next(100) + 10000;
            for (int i = 0; i < len; i++)
            {
                sb.Append((char)r.Next('0', 'Z'));
            }

            string oldText = sb.ToString();

            // Generate random newText.
            len = r.Next(100) + 10000;
            for (int i = 0; i < len; i++)
            {
                sb.Append((char)r.Next('0', 'Z'));
            }

            string newText = sb.ToString();

            Assert.IsTrue(GetDiffTest(oldText, newText));
        }

        private bool GetDiffTest(string oldText, string newText)
        {

            var result = DiffLogic.GetDiff(oldText, newText);

            int diffCount = 0;

            foreach (var r in result)
            {
                if (r.DiffType == Definition.DIFF_TYPE.DIFF_ADD)
                {
                    // ex.)
                    //      old :  abc
                    //            ^ Insert before [1]st char => index = 0
                    //      new : 1abc
                    //
                    oldText = oldText.Insert(r.OldDataIndex + diffCount, r.DiffObjcet.ToString());
                    diffCount += r.DiffObjcet.ToString().Length;
                }
                else if (r.DiffType == Definition.DIFF_TYPE.DIFF_DEL)
                {
                    // ex.)
                    //      old : abc
                    //            ^ Delete [1]st char => index = 1
                    //      new :  bc
                    //
                    oldText = oldText.Remove(r.OldDataIndex - 1 + diffCount, r.DiffObjcet.ToString().Length);
                    diffCount -= r.DiffObjcet.ToString().Length;
                }
            }

            return (newText == oldText);
        }
    }
}