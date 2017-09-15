using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace AutoMapperDemo
{
    /// <summary>
    /// 自定义类型转换器的Demo和自定义解析器的Demo
    /// </summary>
    class CustomMapDemo
    {
        static void Main2(string[] args)
        //static void Main(string[] args)
        {
            CustomMapDemo demo = new CustomMapDemo();

            //自定义类型转换器的Demo
            demo.CustomTypeDemo();

            //自定义解析器的Demo
            demo.CustomValueDemo();
        }

        public void CustomTypeDemo()
        {
            Source2 source = new Source2
            {
                FltConstructFee = 100.1m,
                FuelCostFee = 100.2m,
                TaxFee = 100.3m,
                DateString = "08/22/2016",
                Name = "朱自清"
            }; 
            
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Source2, Destination2>().ConvertUsing(new DateTimeTypeConverter());
            });
            Destination2 destination = Mapper.Map<Source2, Destination2>(source);
        }

        public void CustomValueDemo()
        {
            Source2 source = new Source2
            {
                FltConstructFee = 100.1m,
                FuelCostFee = 100.2m,
                TaxFee = 100.3m,
                DateString = "08/22/2016",
                Name = "朱自清"
            };

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Source2, Destination2>()  //自定义解析器
                    .ForMember(dest => dest.TotalWithTax, opt => opt.ResolveUsing<CustomResolver>());                                                    ;
            });
            Destination2 destination = Mapper.Map<Source2, Destination2>(source);
        }
    }    

    //自定义类型转换器：
    public class DateTimeTypeConverter : ITypeConverter<Source2, Destination2>
    {
        public Destination2 Convert(Source2 source, Destination2 destination, ResolutionContext context)
        {
            return new Destination2 
            { 
                Date = System.Convert.ToDateTime(source.DateString)
            };
        }
    }

    //自定义解析器：
    public class CustomResolver : IValueResolver<Source2, Destination2, decimal>
    {
        public decimal Resolve(Source2 source, Destination2 destination, decimal number, ResolutionContext context)
        {
            return source.FltConstructFee + source.FuelCostFee + source.TaxFee;
        }
    }

    public class Source2
    {
        public decimal FltConstructFee { get; set; }//目标中没有FltConstructFee

        public decimal FuelCostFee { get; set; }//目标中没有FuelCostFee

        public decimal TaxFee { get; set; }//目标中没有TaxFee

        public string DateString { get; set; }//目标中没有DateString，但有DateTime类型的Date成员

        public string Name { get; set; }
    }

    public class Destination2
    {
        public decimal TotalWithTax { get; set; }//源中没有TotalWithTax 

        public DateTime Date { get; set; }//源中没有Date，但有string类型的DateString

        public string Name { get; set; }
    }
}
