using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MoreLinq;
using NUnit.Framework;
using Autofac;

namespace SingletonPattern
{
    public interface IDatabase
    {
        int GetPopulation(string name);
    }
    public class SingletonDatabse : IDatabase
    {
        private Dictionary<string, int> capitals;
        private static int InstanceCount;
        public static int count => InstanceCount;
        private SingletonDatabse()
        {
            InstanceCount++;
            capitals = File.ReadAllLines(
                Path.Combine(
                    new FileInfo(typeof(IDatabase).Assembly.Location).DirectoryName,
                "capitals.txt"))
                .Batch(2)
                .ToDictionary(
                list => list.ElementAt(0).Trim(),
                list => int.Parse(list.ElementAt(1)));                
        }
        public int GetPopulation(string name)
        {
            return capitals[name];
        }
        private static Lazy<SingletonDatabse> instance = new Lazy<SingletonDatabse>(() => new SingletonDatabse() );
        public static SingletonDatabse Instance = instance.Value;
    }
    public class OrdinaryDatabase : IDatabase
    {
        private Dictionary<string, int> capitals;
        private OrdinaryDatabase()
        {
            capitals = File.ReadAllLines(
                Path.Combine(
                    new FileInfo(typeof(IDatabase).Assembly.Location).DirectoryName,
                "capitals.txt"))
                .Batch(2)
                .ToDictionary(
                list => list.ElementAt(0).Trim(),
                list => int.Parse(list.ElementAt(1)));
        }
        public int GetPopulation(string name)
        {
            return capitals[name];
        }
    }
    public class SingletonRecordFinder
    {
        public int GetTotalPopulation(IEnumerable<string> names)
        {
            int result = 0;
            foreach(var name in names)
            {
                result += SingletonDatabse.Instance.GetPopulation(name);
            }
            return result;
        }


    }
   
    public class ConfigurableRecordFinder
    {
        private IDatabase database;
        public ConfigurableRecordFinder(IDatabase database)
        {
            this.database = database ?? throw new ArgumentNullException(paramName:nameof(database)); 
        }
        public int GetTotalPopulation(IEnumerable<string> names)
        {
            int result = 0;
            foreach (var name in names)
            {
                result += database.GetPopulation(name);
            }
            return result;
        }
    }
    public class DummyDatabase : IDatabase
    {
        public int GetPopulation(string name)
        {
            return new Dictionary<string, int>
            {
                {"Alpha",1 },
                {"Beta",2 },
                {"Gamma",3 }
            }[name];
        }
    }
    [TestFixture]
    public class SingletonTests
    {

        [Test]
        public void IsSingletonTest()
        {
            var db = SingletonDatabse.Instance;
            var db2 = SingletonDatabse.Instance;
            Assert.That(db, Is.SameAs(db2)); // check both refer to same object
            Assert.That(SingletonDatabse.count, Is.EqualTo(1));
        }
        [Test]
        public void GetTotalPopulationTest()
        {
            var db = new SingletonRecordFinder();
            string[] city = new[] {"Seoul","Mexico City"};
            int totalPopulation = db.GetTotalPopulation(city);
            Assert.That(totalPopulation, Is.EqualTo(17400000 + 17500000));
        }
        [Test]
        public void ConfigurablePopulationTests()
        {
            var rf = new ConfigurableRecordFinder(new DummyDatabase());
            var names = new string[] {"Alpha","Gamma"};
            var tp = rf.GetTotalPopulation(names);
            Assert.That(tp, Is.EqualTo(4));
        }
        [Test]
        public void DIPopulationTest()
        {
            var cb = new ContainerBuilder();
            cb.RegisterType<OrdinaryDatabase>().As<IDatabase>().SingleInstance();
            cb.RegisterType<ConfigurableRecordFinder>();
            using (var c = cb.Build())
            {
                var rf = c.Resolve<ConfigurableRecordFinder>();
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var db = SingletonDatabse.Instance;
            var city = "Tokyo";
            Console.WriteLine($"{city} has population of {db.GetPopulation(city)}");
            Console.ReadLine();
        }
    }
}