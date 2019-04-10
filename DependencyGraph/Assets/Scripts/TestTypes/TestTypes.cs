using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestTypes
{
    public class TestA
    {
        public TestB _testB;
        public TestC _testC;
        public TestD _testD;
        public TestE _testE;
        public TestF _testF;
    }

    public class TestB
    {
        public TestE _testE;
    }

    public class TestC
    {
        public TestF _testF;
    }

    public class TestD
    {
        public TestA _testA;
    }

    public class TestE
    {
        
    }

    public class TestF
    {
        
    }
}
