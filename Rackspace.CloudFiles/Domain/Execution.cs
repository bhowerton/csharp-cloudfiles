
using System;
using Rackspace.CloudFiles.utils;

namespace Rackspace.CloudFiles.Domain
{
    public static class StartProcess 
    {
		
        public static Execution<TR> ByDoing<TR>(Func<TR> startaction)
        {
            return new Execution<TR> (startaction);
        }
        public static Execution ByDoing(Action action)
        {
            return new Execution(action);
        }
    }
    public class Execution<TR>{
        private Func<TR> _action;	
        public Error<TR,T> AndIfErrorThrownIs<T>() where T: Exception
        {
            return new Error<TR,T>(_action);
        }
        public Execution(Func<TR> action){
			
            _action = action;
			
        }
    }
    public class Execution
    {
        private Action _action;
	 
		
        public Error<T> AndIfErrorThrownIs<T>() where T: Exception
        {
            return new Error<T>(_action);
        }
        public Execution(Action action){
			
            _action = action;
			
        }
				
    }

    public class Error<R,T> where T: Exception
    {	
        public R Do(Action<T> erroraction)
        {
            try
            {
                return _startaction.Invoke();
            }
            catch (T ex)
            {
                erroraction.Invoke(ex);

                throw;
            }
        }
		
        private Func<R> _startaction;
		
		
        public Error(Func<R> startaction)
        {
            _startaction = startaction;
			
        }
    }

    public class Error<T> where T: Exception
    {	
        public void Do(Action<T> erroraction)
        {
            try
            {
                _startaction.Invoke();
            }
            catch (T ex)
            {
                erroraction.Invoke(ex);

                throw;
            }
        }
		
        private readonly Action _startaction;
		
		
        public Error(Action startaction)
        {
            _startaction = startaction;
			
        }
    }
}