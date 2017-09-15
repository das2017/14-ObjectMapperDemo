using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using AutoMapper;

namespace AutoMapperDemo
{
    class BasicUsageDemo
    {
        //static void Main1(string[] args)
        static void Main(string[] args)
        {
            BasicUsageDemo demo = new BasicUsageDemo();

            //最基本的用法、扁平化映射：
            demo.BasicMapDemo(); 

            //其他用法的Demo：
            //demo.AdvancedMapDemo();       

            //动态对象映射：
            //demo.DynamicMapDemo(); 
        }

        public void BasicMapDemo()
        {
            Source source = new Source()
            {
                ID = 1,
                Name = "朱自清",
                MemberNumber = "123456",
                Height = 150,
                Value = 260,
                Customer = new Customer()
                {
                    Name = "中青易游"
                },
                CustomerList = new List<Customer>
                {
                    new Customer
                    {
                        Name = "第1位客户"
                    },
                    new Customer
                    {
                        Name = "第2位客户"
                    }
                }
            };

            //写法一：          
            MapperConfiguration config = new MapperConfiguration(cfg => cfg.CreateMap<Source, Destination>());
            IMapper mapper = config.CreateMapper();
            Destination destination1 = mapper.Map<Destination>(source);

            //写法二（推荐此写法）：
            Mapper.Initialize(cfg => cfg.CreateMap<Source, Destination>());
            Destination destination2 = Mapper.Map<Destination>(source);

            //写法三：利用Profile
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<SourceProfile>();
            });
            Destination destination3 = Mapper.Map<Destination>(source);
        }                            

        public void AdvancedMapDemo()
        {
            Source source = new Source()
            {
                ID = 1,
                Name = "朱自清",
                //Height = 150,
                Height = 151,
                Value = 260,
                FromGroup = "作家组",
                MainAirlinaName = "中国南方航空公司",
                Date = new DateTime(2016, 11, 22, 10, 30, 0),
                Customer = new Customer()
                {
                    Name = "第1位客户"
                }
            };

            Mapper.Initialize(cfg =>
            {
                cfg.RecognizePrefixes("From");  //表示源对象中的From***成员变量值能够映射到目标对象中***成员变量，如：源对象中的FromGroup成员将映射到目标对象中的Group成员
                cfg.ReplaceMemberName("Airlina", "Airline"); //表示源对象中的含有Airlina的成员变量映射到目标对象中含有Airline的成员变量      
                cfg.CreateMap<Source, Destination>()
                    .ForMember(dest => dest.MemberNumber, opt =>   //空值替换
                    {
                        opt.NullSubstitute("654321");
                    })
                  .ForMember(dest => dest.CustomerName, opt => opt.Ignore()) //忽略CustomerName字段不被映射
                  .ForMember(dest => dest.Height, opt => opt.Condition(src => (src.Height > 150))) //条件映射
                  .ForMember(dest => dest.EventDate, opt => opt.MapFrom(src => src.Date.Date)) //指定字段映射
                  .ForMember(dest => dest.EventHour, opt => opt.MapFrom(src => src.Date.Hour))
                  .ForMember(dest => dest.EventMinute, opt => opt.MapFrom(src => src.Date.Minute))
                  .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer.Name));                  
            });            
            Destination destination = Mapper.Map<Source, Destination>(source, opt =>
            {
                opt.BeforeMap((src, dest) => src.Height = src.Height + 10); //前映射
                opt.AfterMap((src, dest) => dest.CustomerName = "季羡林");  //后映射
            });
        }        

        public void DynamicMapDemo()
        {
            Source source = new Source()
            {
                Height = 150,
                Value = 100
            };

            Mapper.Initialize(cfg => { });

            //Source对象映射动态对象：
            dynamic result1 = Mapper.Map<ExpandoObject>(source);
            Console.WriteLine(result1.Height);
            Console.WriteLine(result1.Value);          

            //动态对象映射Destination对象：
            dynamic dynamicSource = new ExpandoObject();
            dynamicSource.Height = 5;
            dynamicSource.Value = 6;

            Destination result2 = Mapper.Map<Destination>(dynamicSource);           
        }
    }

    public class SourceProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<Source, Destination>();
        }
    }

    public class Source
    {
        public int ID { get; set; }

        public string Name { get; set; } //目标中没有Name

        public string MemberNumber { get; set; }

        public int Height { get; set; }

        public int Value { get; set; }

        public string FromGroup { get; set; } //目标中没有FromGroup，但有Group成员

        public string MainAirlinaName { get; set; } //目标中没有MainAirlinaName，但有MainAirlineName

        public DateTime Date { get; set; }//目标中没有Date

        public Customer Customer { get; set; } //目标中没有Customer对象，但有个同名的string类型变量成员

        public List<Customer> CustomerList { get; set; }

        public decimal GetTotalWithoutTax()
        {
            return 1088.10m;
        }
    }

    public class Destination
    {
        public int ID { get; set; }

        public int MemberNumber { get; set; }  //源中成员MemberNumber的类型是string

        public uint Height { get; set; } //源中成员Height的类型是int

        public int Value { get; set; }

        public string Group { get; set; } //源中没有Group，但有FromGroup字段

        public string MainAirlineName { get; set; } //源中没有MainAirlineName，但有MainAirlinaName

        public DateTime EventDate { get; set; } //源中没有EventDate

        public int EventHour { get; set; }//源中没有EventHour

        public int EventMinute { get; set; } //源中没有EventMinute

        public decimal TotalWithTax { get; set; }//源中没有TotalWithTax

        public string CustomerName { get; set; }//源中没有CustomerName

        public string TotalWithoutTax { get; set; }//源中没有TotalWithoutTax

        public string Customer { get; set; } //源中成员Customer的类型是类Customer

        public List<Customer> CustomerList { get; set; }
    }

    public class Customer
    {
        public string Name { get; set; }
    }    
}
