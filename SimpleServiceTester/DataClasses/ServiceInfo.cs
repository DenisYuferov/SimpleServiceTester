using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Controls;

namespace SimpleServiceTester.DataClasses
{
    public class ServiceInfo
    {
        public MethodInfo MethodInfo;
        public string ServiceUrl, InterfaceName;
        //public TreeViewItem MethodNode;
        public List<Type> Types;
    }
}
