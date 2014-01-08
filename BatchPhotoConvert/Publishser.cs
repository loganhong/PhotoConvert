using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BatchPhotoConvert
{
    //public delegate void NumberChangedEventHandler(int count);
    //public delegate string GeneralEventHandler();
    //public delegate string DemoEventHandler(int num);
    public class Publishser
    {
        public event EventHandler MyEvent;
        private int count;
        //public NumberChangedEventHandler NumberChanged;
        //public event NumberChangedEventHandler NumberChanged;
        //private event DemoEventHandler numberChanged;

        //public event DemoEventHandler NumberChanged
        //{
        //    add
        //    {
        //        numberChanged += value;
        //    }
        //    remove
        //    {
        //        numberChanged -= value;
        //    }
        //}

        public void DoSomething()
        {
            if (MyEvent != null)
            {
                //try
                //{
                //    MyEvent(this, EventArgs.Empty);
                //}
                //catch (Exception e)
                //{

                //    throw;
                //}
                Delegate[] delArray = MyEvent.GetInvocationList();
                foreach (Delegate item in delArray)
                {
                    //EventHandler method = item as EventHandler;
                    try
                    {
                        //method(this, EventArgs.Empty);
                        item.DynamicInvoke(this, EventArgs.Empty);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message);
                    }
                }
            }
            //List<string> strList = new List<string>();

            //if (numberChanged != null)
            //{
            //    Delegate[] delArray = numberChanged.GetInvocationList();
            //    foreach (Delegate del in delArray)
            //    {
            //        DemoEventHandler method = del as DemoEventHandler;
            //        strList.Add(method(100));
            //    }
            //}
            //return strList;
        }

        private static object[] FireEvent(Delegate del, params object[] args)
        {
            List<object> objList = new List<object>();

            if (del != null)
            {
                Delegate[] delArray = del.GetInvocationList();
                foreach (Delegate method in delArray)
                {
                    try
                    {
                        object obj = method.DynamicInvoke(args);
                        if (obj != null)
                        {
                            objList.Add(obj);
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            }
            return objList.ToArray();
        }
    }
}
