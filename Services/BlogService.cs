using IrvineReview.Domain;
using IrvineReview.Services;
using IrvinewReview.Models.Requests;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WikiDataProvider.Data.Extensions;

namespace IrvinewReview.Services
{
    public class BlogService : BaseService
    {
        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public List<MapDomain> CreateMapRadiusBuyers(BlogInsertRequest model)
        {
            List<MapDomain> mapList = null;

            try
            {
                DataProvider.ExecuteCmd(GetConnection, "dbo.AddressTest_Find"
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {
                  paramCollection.AddWithValue("@latpoint", model.Latpoint);
                  paramCollection.AddWithValue("@lngpoint", model.Lngpoint);
                  paramCollection.AddWithValue("@radius", model.Radius);
              }, map: delegate (IDataReader reader, short set)
              {
                  var singleMap = new MapDomain();
                  int startingIndex = 0; //startingOrdinal

                  singleMap.MapId = reader.GetSafeInt32(startingIndex++);
                  singleMap.Address1 = reader.GetSafeString(startingIndex++);
                  singleMap.Address2 = reader.GetSafeString(startingIndex++);
                  singleMap.City = reader.GetSafeString(startingIndex++);
                  singleMap.State = reader.GetSafeString(startingIndex++);
                  singleMap.ZipCode = reader.GetSafeString(startingIndex++);
                  singleMap.Latitude = reader.GetSafeDecimal(startingIndex++);
                  singleMap.Longitude = reader.GetSafeDecimal(startingIndex++);
                  singleMap.DistanceInMile = reader.GetSafeDouble(startingIndex++);

                  if (mapList == null)
                  {
                      mapList = new List<MapDomain>();
                  }

                  mapList.Add(singleMap);
              });
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return mapList;
        }
    }
}