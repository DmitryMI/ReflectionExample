using System;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace ConsoleApp1
{

    public class RefType
    {
        public RefType()
        {
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] valueBytes = new byte[sizeof(int)];
            rng.GetBytes(valueBytes);
            Value = BitConverter.ToInt32(valueBytes, 0);
        }

        public int Value { get; }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class SomeClass
    {
        private int[] _arrayInt;
        private double[] _arrayDouble;
        private char[] _arrayChar;
        private object[] _arrayRef;

        public SomeClass()
        {
            _arrayInt = new int[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
            _arrayDouble = new double []{0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
            _arrayChar = new char[]{'a','b','c'};
            _arrayRef = new object[]{new RefType(), new RefType(), new RefType()};
        }

        public int[] ArrayInt
        {
            get => _arrayInt;
            set => _arrayInt = value;
        }

        public double[] ArrayDouble
        {
            get => _arrayDouble;
            set => _arrayDouble = value;
        }

        public char[] ArrayChar
        {
            get => _arrayChar;
            set => _arrayChar = value;
        }

        public object[] ArrayRef
        {
            get => _arrayRef;
            set => _arrayRef = value;
        }

        private static string ArrayToString<T>(T[] array)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < array.Length; i++)
            {
                builder.Append(array[i]).Append(' ');
            }

            return builder.ToString();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Object contents:\n");
            builder.Append(ArrayToString(_arrayInt)).Append('\n');
            builder.Append(ArrayToString(_arrayDouble)).Append('\n');
            builder.Append(ArrayToString(_arrayChar)).Append('\n');
            builder.Append(ArrayToString(_arrayRef)).Append('\n');
            return builder.ToString();
        }

        public static void CompareRefs(SomeClass instanceA, SomeClass instanceB)
        {
            Console.WriteLine(instanceA._arrayInt == instanceB._arrayInt);
            Console.WriteLine(instanceA._arrayDouble == instanceB._arrayDouble);
            Console.WriteLine(instanceA._arrayChar == instanceB._arrayChar);
            Console.WriteLine(instanceA._arrayRef == instanceB._arrayRef);
        }
    }
    

    public static class Program
    {
        private static void Main(string[] args)
        {
            SomeClass originalInstance = new SomeClass();
            SomeClass cloneInstance = new SomeClass();

            Type type = originalInstance.GetType();
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var fieldInfo in fieldInfos)
            {
                Type fieldType = fieldInfo.FieldType;
                if (fieldType.IsArray)
                {
                    Console.WriteLine($"Field {fieldInfo.Name} is of type {fieldType}");
                    Array arrayObject = (Array) fieldInfo.GetValue(originalInstance);
                    if (arrayObject == null)
                    {
                        fieldInfo.SetValue(cloneInstance, null);
                    }
                    else
                    {
                        Array arrayClone = (Array)Activator.CreateInstance(fieldType, arrayObject.Length);
                        if (arrayClone == null)
                        {
                            throw new InvalidOperationException("Shit happened");
                        }
                        Array.Copy(arrayObject, arrayClone, arrayObject.Length);
                    }
                }
            }

            Console.WriteLine("Original: \n" + originalInstance.ToString());
            Console.WriteLine("Clone: \n" + originalInstance.ToString());
            
            Console.WriteLine("Comparing refs:");
            SomeClass.CompareRefs(originalInstance, cloneInstance);
            Console.ReadKey();
        }
    }
}