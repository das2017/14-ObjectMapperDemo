using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmitMapper;
using EmitMapper.MappingConfiguration;

namespace EmitMapperDemo
{
    class ConvertGenericDemo
    {
        //static void Main3(string[] args)
        static void Main(string[] args)
        {
            ConvertGenericDemo demo = new ConvertGenericDemo();

            demo.ConvertGenericDemo1();           

            Console.ReadLine();
        }

        public void ConvertGenericDemo1()
        {
            ObjectsMapper<Source, Destination> mapper = ObjectMapperManager
                .DefaultInstance
                 .GetMapper<Source, Destination>(
                    //new DefaultMapConfig()
                    new CustomMapConfig()
                    .ConvertGeneric(
                        typeof(HashSet<>),
                        typeof(List<>),
                        //new DefaultCustomConverterProvider(typeof(HashSetToListConverter<>))
                        new CustomConverterProvider()
                    )
                );

            Source source = new Source();
            Destination destination = null;
            destination = mapper.Map(source);
            //Assert.IsNotNull(destination);
            //Assert.IsNotNull(destination.HashSetList);
            //Assert.AreEqual(3, destination.HashSetList.Count);
            //Assert.AreEqual(source.HashSetList.ElementAt(0), destination.HashSetList.ElementAt(0));
            //Assert.AreEqual(source.HashSetList.ElementAt(1), destination.HashSetList.ElementAt(1));
            //Assert.AreEqual(source.HashSetList.ElementAt(2), destination.HashSetList.ElementAt(2));
        }
    }

    public class Source
    {
        public HashSet<decimal> HashSetList { get; set; }

        public Source()
        {
            HashSetList = new HashSet<decimal>();
            HashSetList.Add(2M);
            HashSetList.Add(1M);
            HashSetList.Add(8M);
        }
    }

    public class Destination
    {
        //public HashSet<decimal> HashSetList { get; set; }
        public List<decimal> HashSetList { get; set; }
    }

    public class CustomMapConfig: DefaultMapConfig
    {
        public CustomMapConfig():base()
		{
			RegisterDefaultCollectionConverters();
		}

        protected override void RegisterDefaultCollectionConverters()
        {
            //ConvertGeneric(typeof(ICollection<>), typeof(Array), new DefaultCustomConverterProvider(typeof(HashSetToListConverter<>)));
            //ConvertGeneric(typeof(HashSet<>), typeof(List<>), new CustomConverterProvider(typeof(HashSetToListConverter<>)));
            ConvertGeneric(typeof(HashSet<>), typeof(List<>), new CustomConverterProvider());
        }
    }

    public class HashSetToListConverter<T>
    {
        public List<T> Convert(HashSet<T> from, object state)
        {
            if (from == null)
            {
                return null;
            }
            return from.ToList();
        }
    }

    public class CustomConverterProvider : ICustomConverterProvider
    {
        public CustomConverterDescriptor GetCustomConverterDescr(
            Type from,
            Type to,
            MapConfigBaseImpl mappingConfig)
        {
            var tFromTypeArgs = DefaultCustomConverterProvider.GetGenericArguments(from);
            var tToTypeArgs = DefaultCustomConverterProvider.GetGenericArguments(to);
            if (tFromTypeArgs == null || tToTypeArgs == null || tFromTypeArgs.Length != 1 || tToTypeArgs.Length != 1)
            {
                return null;
            }
            var tFrom = tFromTypeArgs[0];
            var tTo = tToTypeArgs[0];
            if (tFrom == tTo && (tFrom.IsValueType || mappingConfig.GetRootMappingOperation(tFrom, tTo).ShallowCopy))
            {
                return new CustomConverterDescriptor
                {
                    ConversionMethodName = "Convert",
                    ConverterImplementation = typeof(HashSetToListConverter<>),
                    ConverterClassTypeArguments = new[] { tFrom }
                };
            }

            return null;
            //return new CustomConverterDescriptor
            //{
            //    ConversionMethodName = "Convert",
            //    ConverterImplementation = typeof(ArraysConverter_DifferentTypes<,>),
            //    ConverterClassTypeArguments = new[] { tFrom, tTo }
            //};

        }
    }
}
