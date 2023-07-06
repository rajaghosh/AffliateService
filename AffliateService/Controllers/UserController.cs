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
    public class UserController : ControllerBase
    {
        private readonly IDapper _dapper;
        public UserController(IDapper dapper)
        {
            _dapper = dapper;
        }



        [HttpPost("AddNewUser")]
        public async Task<ResponseModel<int>> InsertNewUserDetail([FromBody] UserRegistrationModel userRegModel)
        {
            ResponseModel<int> response = null;

            string name = String.IsNullOrEmpty(userRegModel.Name) ? "" : userRegModel.Name;
            string email = String.IsNullOrEmpty(userRegModel.Email) ? "" : userRegModel.Email;
            string password = String.IsNullOrEmpty(userRegModel.Password) ? "" : userRegModel.Password;

            try
            {
                string Query = $"SELECT ID FROM UserDetails WHERE Email = '{email}'";

                var result = await _dapper.GetAsync<int>(Query, null, System.Data.CommandType.Text);

                if (result >= 1)
                {
                    throw new Exception("User Email Id Already Registered");
                }

                Query = "";

                Query = @"Insert into UserDetails(Name, Email, Password) ";
                Query = Query + @" values('" + name + "', '" + email + "', '" + password + "') ";

                //result = await _dapper.Insert<int>(Query, null, System.Data.CommandType.Text);

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

        [HttpPost("LoginUser")]
        public async Task<ResponseModel<string>> LoginUserDetail([FromBody] UserRegistrationModel userRegModel)
        {
            ResponseModel<string> response = new ResponseModel<string>();

            string name = String.IsNullOrEmpty(userRegModel.Name) ? "" : userRegModel.Name;
            string email = String.IsNullOrEmpty(userRegModel.Email) ? "" : userRegModel.Email;
            string password = String.IsNullOrEmpty(userRegModel.Password) ? "" : userRegModel.Password;

            string token = string.Empty;

            try
            {
                string Query = $"SELECT ID FROM UserDetails " +
                                $"WHERE Email = '{email}' " +
                                $"AND Password = '{password}' ";

                var result = await _dapper.GetAsync<int>(Query, null, System.Data.CommandType.Text);

                if (result >= 1)
                {
                    token = "User exists";
                    response.Data = token;
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
            catch (Exception ex)
            {
                response = new ResponseModel<string>();
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
