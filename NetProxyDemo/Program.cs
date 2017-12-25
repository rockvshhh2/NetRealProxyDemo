using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Services;
using System.Text;
using System.Threading.Tasks;

namespace NetProxyDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            被代理對象 被代理對象的執行個體 = new 被代理對象();

            真實代理 代理 = new 真實代理(被代理對象的執行個體);

            object 取得透明代理 = 代理.GetTransparentProxy();

            被代理對象 取得實際被透明代理所代理的類別 = 取得透明代理 as 被代理對象;

            測試被當成參數的類別 testPar = new 測試被當成參數的類別();

            string testOutString = "";

            取得實際被透明代理所代理的類別.sayHello(testPar,out testOutString);

            Console.WriteLine("代理回應訊息完成");

            Console.ReadKey();
        }
    }

    public class 真實代理 : RealProxy //需繼承RealProxy類別
    {
        private readonly object _target; //被代理的目標物件

        public 真實代理(object target)
            : base(target.GetType())
        {
            Console.WriteLine("建立代理！");

            _target = target;
        }

        public override IMessage Invoke(IMessage msg)
        {
            Console.WriteLine("代理接收到訊息！");

            //將訊息轉換成方法訊息
            var methodCall = msg as IMethodCallMessage; 

            //取得所傳入的參數
            var pars = methodCall.Args;

            Console.WriteLine("代理呼叫目標方法！");

            //呼叫方法
            var result = methodCall.MethodBase.Invoke(_target, pars);

            Console.WriteLine("代理呼叫目標方法完成！");

            Console.WriteLine("代理回應訊息！");

            //回應訊息
            return new ReturnMessage(result, pars, pars.Count(), methodCall.LogicalCallContext, methodCall);
        }
    }

    public class 被代理對象
        : MarshalByRefObject //繼承此類別的用意是，啟用此類別是可以被遠程代理的
    {
        public 被代理對象()
        {
            Console.WriteLine("被代理對象建立！");
        }

        // 測試傳入的參數類型
        public string sayHello(測試被當成參數的類別 d,out string testString)
        {
            Console.WriteLine("sayHello 被呼叫！");

            testString = "Test Out Parameter";

            d.MyProperty = 123456;

            Console.WriteLine("sayHello 執行完成！");

            return "Demo !";
        }
    }

    public class 測試被當成參數的類別
    {
        public int MyProperty { get; set; }
    }
}
