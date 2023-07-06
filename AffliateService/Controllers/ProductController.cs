using AffliateService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telemedicine.Data;
using Telemedicine.Models;

namespace AffliateService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IDapper _dapper;
        public ProductController(IDapper dapper)
        {
            _dapper = dapper;
        }

        [HttpGet("GetProductDetails")]
        public async Task<ResponseModel<List<ProductDetailsBasic>>> GetProductDetails(string userEmail)
        {
            ResponseModel<List<ProductDetailsBasic>> response = new ResponseModel<List<ProductDetailsBasic>>();

            bool isAdmin = false;

            try
            {
                //Check if the user has admin role
                string Query = $"SELECT ID FROM UserDetails " +
                                $"WHERE Email = '{userEmail}' " +
                                $"AND IsAdmin = 1 ";

                var result1 = await _dapper.GetAsync<int>(Query, null, System.Data.CommandType.Text);

                if (result1 >= 1)
                {
                    isAdmin = true;
                }

                if (isAdmin)
                {
                    Query = "";

                    Query = $"SELECT Id, MedicineName, ImagePath FROM Product";

                    var result2 = await _dapper.GetAllAsync<ProductDetailsBasic>(Query, null, System.Data.CommandType.Text, "UseMarketPlaceDB");

                    if (result2.Count >= 1)
                    {
                        response.Data = result2;
                    }
                    else
                    {
                        response.Error = new ErrorModel()
                        {
                            ErrorCode = ErrorCode.NotFound,
                            ErrorMessage = "Data Not Found."
                        };
                    }
                }

            }
            catch (Exception ex)
            {
                response = new ResponseModel<List<ProductDetailsBasic>>();
                response.Error = new ErrorModel()
                {
                    ErrorCode = ErrorCode.InternalServerError,
                    ErrorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message
                };
            }

            return response;
        }

        [HttpPost("AddDiscountPercent")]
        public async Task<ResponseModel<int>> InsertDiscountPercent([FromBody] ProductDiscountPayload discountModel)
        {
            ResponseModel<int> response = null;

            int prodId = discountModel.ProductId;
            decimal discountPercent = discountModel.DiscountPercent;
            //string password = String.IsNullOrEmpty(userRegModel.Password) ? "" : userRegModel.Password;

            try
            {
                string Query = $"SELECT ID FROM ProductAffliateDiscount WHERE ProductId = '{prodId}'";

                var result = await _dapper.GetAsync<int>(Query, null, System.Data.CommandType.Text);

                if (result >= 1)
                {

                    Query = "";

                    Query = $"UPDATE ProductAffliateDiscount " +
                            $"SET DiscountPercent = {discountPercent} " +
                            $"WHERE ProductId = {prodId} ";


                    response = new ResponseModel<int>();
                    result = await _dapper.Update<int>(Query, null, System.Data.CommandType.Text);
                    if (result >= 0)
                    {
                        response.Data = result;
                    }
                    else
                    {
                        response.Error = new ErrorModel()
                        {
                            ErrorCode = ErrorCode.NotFound,
                            ErrorMessage = "Data Not Found."
                        };
                    }
                }
                else
                {
                    Query = "";

                    Query = @"Insert into ProductAffliateDiscount(ProductId, DiscountPercent) ";
                    Query = Query + @" values(" + prodId + ", " + discountPercent + ") ";


                    response = new ResponseModel<int>();
                    result = await _dapper.Insert<int>(Query, null, System.Data.CommandType.Text);
                    if (result >= 0)
                    {
                        response.Data = result;
                    }
                    else
                    {
                        response.Error = new ErrorModel()
                        {
                            ErrorCode = ErrorCode.NotFound,
                            ErrorMessage = "Data Not Found."
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                response = new ResponseModel<int>();
                response.Error = new ErrorModel()
                {
                    ErrorCode = ErrorCode.InternalServerError,
                    ErrorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message
                };
            }

            return response;
        }


    }
}
