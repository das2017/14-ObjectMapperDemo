using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EmitMapper;

namespace EmitMapperDemo
{
    /// <summary>
    /// 1、EmitMapper的基本用法的Demo
    /// </summary>
    public class BasicUsageDemo
    {
        static void Main(string[] args)
        //static void Main1(string[] args)
        {
            Source source = new Source
            {
                Name = "Name来自源对象",
                Number = 1,
                Price = 5.8m,
                //Description = "Description来自源对象",
                TicketNoStatus = StatusEnum.A,
                OrderNoStatus = "3",
                PartStruct = new Source.InnerStruct
                {
                    InnerName = "InnerName来自源对象的内部结构InnerStruct"
                },
                PartClass = new Source.InnerClass
                {
                    InnerName = "InnerName来自源对象的内部类InnerClass"
                },
                GroupArray = new Source.SourceInnerClass[2],
                UpdatedTime = DateTime.Now
            };
            source.GroupArray[0] = new Source.SourceInnerClass();
            source.GroupArray[0].InnerName = "来自源对象的零件1";
            source.GroupArray[1] = new Source.SourceInnerClass();
            source.GroupArray[1].InnerName = "来自源对象的零件2";

            ObjectsMapper<Source, Destination> mapper = ObjectMapperManager.DefaultInstance.GetMapper<Source, Destination>();
            Destination destination = mapper.Map(source);

            Destination destination2 = new Destination();
            ObjectMapperManager.DefaultInstance.GetMapper<Source, Destination>().Map(source, destination2);

            Console.ReadLine();
        }

        public class Source
        {
            public struct InnerStruct
            {
                public string InnerName { get; set; }
            }
            public class InnerClass
            {
                public string InnerName { get; set; }
            }
            public class SourceInnerClass
            {
                public string InnerName { get; set; }
            }

            public string Name { get; set; }

            public string Brand
            {
                get
                {
                    return "Brand来自源对象";
                }
            }

            public int Number { get; set; }

            public decimal? Price { get; set; }

            public object Description { get; set; }

            public StatusEnum TicketNoStatus { get; set; }

            public string OrderNoStatus { get; set; }

            public InnerStruct PartStruct { get; set; }

            public InnerClass PartClass { get; set; }

            public SourceInnerClass[] GroupArray { get; set; }

            public DateTime UpdatedTime { get; set; }
        }

        public class Destination
        {
            public class InnerStruct //源中的类型是结构，目标的类型是类
            {
                public string InnerName { get; set; }
            }
            public struct InnerClass //源中的类型是类，目标的类型是结构
            {
                public string InnerName { get; set; }
            }
            public class DestinationInnerClass //源中的类名叫SourceInnerClass
            {
                public DestinationInnerClass()
                {
                    intern = 13;
                }

                internal int intern = 13;  //源中里没有intern

                private string _innerName = "InnerName来自目标对象的内部类DestinationInnerClass";
                public string InnerName
                {
                    get
                    {
                        return _innerName;
                    }
                    set
                    {
                        _innerName = value;
                    }
                }
            }

            private string name = "Name来自目标类对象";
            public string Name
            {
                get
                {
                    return name;
                }
                set
                {
                    name = value;
                }
            }

            private string brand = "Brand来自目标类对象";
            public string Brand
            {
                get
                {
                    return brand;
                }
                set
                {
                    brand = value;
                }
            }

            public int Number { get; set; }

            public int Price { get; set; } //源中的类型是decimal?

            private string description = "Description来自目标类对象"; //源中的类型是object 
            public string Description
            {
                get
                {
                    return description;
                }
                set
                {
                    description = value;
                }
            }

            private StatusEnum ticketNoStatus = StatusEnum.C;
            public StatusEnum TicketNoStatus
            {
                get
                {
                    return ticketNoStatus;
                }
                set
                {
                    ticketNoStatus = value;
                }
            }

            public StatusEnum OrderNoStatus { get; set; } //源中的类型是string，目标的类型是枚举

            public InnerStruct PartStruct { get; set; } //源中的类型是结构，目标的类型是类

            public InnerClass PartClass { get; set; } //源中的类型是类，目标的类型是结构

            public DestinationInnerClass[] GroupArray { get; set; } //源中的类名叫SourceInnerClass，目标的类名叫DestinationInnerClass

            public DateTime UpdatedTime { get; set; }
        }

        public enum StatusEnum : byte
        {
            A = 1,
            B = 2,
            C = 3
        }
    }
}
