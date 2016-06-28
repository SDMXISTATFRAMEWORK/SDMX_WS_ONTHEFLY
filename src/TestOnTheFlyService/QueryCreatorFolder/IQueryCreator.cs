using System;
using System.Collections.Generic;
namespace TestOnTheFlyService
{
    public interface IQueryCreator
    {
        event QueryCreator.GetDataEventDelegate GetDataEvent;
        event QueryCreator.voidDelegate ResetEvent;
        event QueryCreator.voidDelegate UpdateFlatEvent;

        void UpdateQuery(SdmxObject _ChoosesDf, System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<SdmxQueryInput>> _DataQuery, List<OrderDim> DimensionOrdered);
        void UpdateQueryThread(object testobj);
    }
}
