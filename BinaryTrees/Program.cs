using NUnit.Framework;
using NUnitLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BinaryTrees
{
    class Program
    {
        static void Main(string[] args)
        {
            RunWithTimeout(MixedTest, 500, "Your tree is too slow");
        }

        public static void RunWithTimeout(Action work, int timeout, string message)
        {
            var task = Task.Run(work);
            if (!task.Wait(timeout))
                throw new AssertionException(message);
        }

        public static void MixedTest()
        {
            var tree = new BinaryTree<int>();
            var indexer = MakeAccessor(tree);
            var type = typeof(BinaryTreeInTask2_should);
            var stream = type.Assembly.GetManifestResourceStream("BinaryTrees.test.txt")
                         ?? throw new Exception("no resource!");
            using (StreamReader sr = new StreamReader(stream))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine() ?? throw new Exception("Command expected!");
                    var values = line.Split();
                    var command = values.First();
                    if (command == "Add")
                    {
                        foreach (var value in values.Skip(1))
                        {
                            tree.Add(int.Parse(value));
                        }
                    }
                    else
                    {
                        var index = int.Parse(values[1]);
                        var value = int.Parse(values[2]);
                        Assert.AreEqual(value, indexer(index));
                    }
                }
            }
        }

        public static PropertyInfo GetIndexer<T>(BinaryTree<T> t)
    where T : IComparable
        {
            return t.GetType()
                .GetProperties()
                .Select(z => new { prop = z, ind = z.GetIndexParameters() })
                .SingleOrDefault(z => z.ind.Length == 1 && z.ind[0].ParameterType == typeof(int))
                ?.prop;
        }

        public static Func<int, T> MakeAccessor<T>(BinaryTree<T> tree)
            where T : IComparable
        {
            var prop = GetIndexer(tree);
            var param = Expression.Parameter(typeof(int));
            return Expression.Lambda<Func<int, T>>(
                    Expression.MakeIndex(Expression.Constant(tree), prop, new[] { param }),
                    param)
                .Compile();
        }
    }
}
