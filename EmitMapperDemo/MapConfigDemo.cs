using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EmitMapper;
using EmitMapper.MappingConfiguration;

namespace EmitMapperDemo
{
    /// <summary>
    /// 2、结合默认映射配置器的各种配置方法的使用Demo
    /// </summary>   
    public class MapConfigDemo
    {
        static void Main2(string[] args)
        //static void Main(string[] args)
        {
            MapConfigDemo dmcDemo = new MapConfigDemo();
            dmcDemo.ConvertUsingDemo();
            dmcDemo.ShallowMapAndDeepMapDemo();
            dmcDemo.OtherDefaultMapConfiguratorDemo();

            Console.ReadLine();
        }

        public void ConvertUsingDemo()
        {
            Source1 source = new Source1
            {
                Price = 2
            };

            ObjectsMapper<Source1, Destination1> mapper =
                ObjectMapperManager.DefaultInstance.GetMapper<Source1, Destination1>(
                    new DefaultMapConfig()
                        .ConvertUsing<object, decimal>(v => 50.1M)
                );
            Destination1 destination = mapper.Map(source);
        }

        public void ShallowMapAndDeepMapDemo()
        {
            //浅映射Demo：
            Source1 source = new Source1
            {
                Price = 2,
                InnerClassA = new InnerClass1
                {
                    SubName1 = "浅映射Demo"
                }
            };
            ObjectsMapper<Source1, Destination1> mapper =
                ObjectMapperManager.DefaultInstance.GetMapper<Source1, Destination1>(
                    new DefaultMapConfig()
                    .ShallowMap<InnerClass1>()
                );            
            Destination1 destination = mapper.Map(source);           

            Console.WriteLine("destination.InnerClassA.SubName1 = {0}.", destination.InnerClassA.SubName1);
            source.InnerClassA.SubName1 = "浅映射Demo_updated";
            Console.WriteLine("destination.InnerClassA.SubName1 = {0}.", destination.InnerClassA.SubName1);

            //深映射Demo：     
            source.InnerClassA.SubName1 = "深映射Demo";
            ObjectsMapper<Source1, Destination1> mapper2 =
                ObjectMapperManager.DefaultInstance.GetMapper<Source1, Destination1>(
                    new DefaultMapConfig()
                         .DeepMap<InnerClass1>()
                );
            destination = mapper2.Map(source);

            Console.WriteLine("destination.InnerClassA.SubName1 = {0}.", destination.InnerClassA.SubName1);
            source.InnerClassA.SubName1 = "深映射Demo_updated";
            Console.WriteLine("destination.InnerClassA.SubName1 = {0}.", destination.InnerClassA.SubName1);
        }

        public void OtherDefaultMapConfiguratorDemo()
        {
            Source2 source = new Source2
            {
                Number = 1,
                UpdatedTime = "2015/6/2 08:49:40.570",
                Name = "Name来自源对象",
                EnglishName = "EnglishName来自源对象"
            };

            ObjectsMapper<Source2, Destination2> mapper =
                ObjectMapperManager.DefaultInstance.GetMapper<Source2, Destination2>(
                    new DefaultMapConfig()
                        .MatchMembers((m1, m2) => "M" + m1 == m2)
                        .ConstructBy<Destination2>(() => new Destination2(6))
                        .IgnoreMembers<Source2, Destination2>(new string[] { "Number" })
                        .NullSubstitution<decimal?, decimal>((value) => -2M)
                        .PostProcess<Destination2>((value, state) => { value.MRemark = "MRemark来自目标对象"; return value; })
                );
            Destination2 destination = mapper.Map(source);
        }

        public class Source1
        {
            public int? Price { get; set; }
            public InnerClass1 InnerClassA { get; set; }
        }

        public class Destination1
        {
            public decimal Price = 100M;
            public InnerClass1 InnerClassA { get; set; }
        }

        public class Source2
        {
            public int Number { get; set; }
            public decimal? Price { get; set; }
            public string UpdatedTime { get; set; }
            public string Name { get; set; }
            public string EnglishName { get; set; }
        }

        public class Destination2
        {
            public Destination2(int i)
            {
                MOrder = i;
            }

            public int? MNumber { get; set; } //源中成员Number的类型是int
            public decimal MPrice { get; set; } //源中成员Price的类型是decimal?
            public DateTime MUpdatedTime { get; set; } //源中成员UpdatedTime的类型是string
            public string MName { get; set; }
            public string MRemark { get; set; } //源中没有MRemark         
            public int MOrder { get; set; }//源中没有MOrder
        }

        public class InnerClass1
        {
            public string SubName1 { get; set; }
        }
    }
}
