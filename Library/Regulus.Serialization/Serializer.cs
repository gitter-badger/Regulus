﻿using System;
using System.Collections.Generic;
using System.Linq;

using Regulus.Remoting;

namespace Regulus.Serialization
{
    public class Serializer : ISerializer
    {
        private readonly ITypeDescriber[] _Describers;

        public Serializer(params ITypeDescriber[] describers)
        {
            _Describers = describers;

            foreach (var typeDescriber in _Describers)
            {
                typeDescriber.SetMap(_Describers);
            }          
        }

        public Serializer(DescriberBuilder describer_builder) : this(describer_builder.Describers)
        {            
        }

        public byte[] ObjectToBuffer(object instance)
        {
            var type = instance.GetType();
           
            return ObjectToBuffer(instance , type);
        }

        public byte[] ObjectToBuffer<T>(T instance)
        {
            return ObjectToBuffer(instance, typeof(T));
        }
        public byte[] ObjectToBuffer(object instance , Type type)
        {
            var describer = _GetDescriber(type);
            var id = describer.Id;
            var idCount = Varint.GetByteCount((ulong)id);
            var bufferCount = describer.GetByteCount(instance);
            var buffer = new byte[idCount + bufferCount];
            var readCount = Varint.NumberToBuffer(buffer, 0, id);
            describer.ToBuffer(instance, buffer, readCount);
            return buffer;
        }


        public T BufferToObject<T>(byte[] buffer)
        {
            return (T)BufferToObject(buffer);
        }
        public object BufferToObject(byte[] buffer)
        {
            ulong id;
            var readIdCount = Varint.BufferToNumber(buffer, 0, out id);
            var describer = _GetDescriber((int)id);
            object instance;
            describer.ToObject(buffer, readIdCount , out instance);

            return instance;
        }

        private ITypeDescriber _GetDescriber(int id)
        {
            return _Describers.First(describer => describer.Id == id);
        }

        private ITypeDescriber _GetDescriber(Type type)
        {
            return _Describers.First(describer => describer.Type == type);
        }

        byte[] ISerializer.Serialize(object instance)
        {
            return ObjectToBuffer(instance);
        }

        object ISerializer.Deserialize(byte[] buffer)
        {
            return BufferToObject(buffer);
        }
    }
}


