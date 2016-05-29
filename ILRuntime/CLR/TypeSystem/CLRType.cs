﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ILRuntime.CLR.Method;
namespace ILRuntime.CLR.TypeSystem
{
    class CLRType : IType
    {
        Type clrType;
        Dictionary<string, List<CLRMethod>> methods;
        ILRuntime.Runtime.Enviorment.AppDomain appdomain;
        List<CLRMethod> constructors;


        public CLRType(Type clrType, Runtime.Enviorment.AppDomain appdomain)
        {
            this.clrType = clrType;
            this.appdomain = appdomain;
        }

        public void Initialize()
        {
            InitializeMethods();
        }
        
        public bool IsGenericInstance
        {
            get
            {
                return false;
            }
        }

        public Type TypeForCLR
        {
            get
            {
                return clrType;
            }
        }

        public string FullName
        {
            get
            {
                return clrType.FullName;
            }
        }

        void InitializeMethods()
        {
            methods = new Dictionary<string, List<CLRMethod>>();
            constructors = new List<CLRMethod>();
            foreach (var i in clrType.GetMethods())
            {
                if (i.IsConstructor)
                {
                    constructors.Add(new CLRMethod(i, this, appdomain));
                }
                else
                {
                    List<CLRMethod> lst;
                    if (!methods.TryGetValue(i.Name, out lst))
                    {
                        lst = new List<CLRMethod>();
                        methods[i.Name] = lst;
                    }
                    lst.Add(new CLRMethod(i, this, appdomain));
                }
            }
        }

        public IMethod GetMethod(string name, int paramCount)
        {
            if (methods == null)
                InitializeMethods();
            List<CLRMethod> lst;
            if (methods.TryGetValue(name, out lst))
            {
                foreach (var i in lst)
                {
                    if (i.ParameterCount == paramCount)
                        return i;
                }
            }
            return null;
        }

        public IMethod GetMethod(string name, List<IType> param)
        {
            return GetMethod(name, param.Count);
        }

        public IMethod GetConstructor(List<IType> param)
        {
            foreach (var i in constructors)
            {
                if (i.ParameterCount == param.Count)
                    return i;
            }
            return null;
        }
    }
}
