using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yodo1.MAS
{
    public class Yodo1U3dAdError
    {
        private int code;
        public int Code
        {
            get
            {
                return code;
            }
            set
            {
                code = value;
            }
        }

        private string message;
        public string Message
        {
            get
            {
                return message;
            }
            set
            {
                message = value;
            }
        }

        public static Yodo1U3dAdError createWithJson(string errorJsonString)
        {
            Yodo1U3dAdError error = new Yodo1U3dAdError();
            Dictionary<string, object> errorDic = (Dictionary<string, object>)Yodo1JSON.Deserialize(errorJsonString);
            if (errorDic != null)
            {
                if (errorDic.ContainsKey("code"))
                {
                    error.Code = int.Parse(errorDic["code"].ToString());
                }
                if (errorDic.ContainsKey("message"))
                {
                    error.Message = errorDic["message"].ToString();
                }
            }
            return error;
        }

        override
        public string ToString()
        {
            Dictionary<string, object> errorDic = new Dictionary<string, object>();
            errorDic.Add("code", Code);
            errorDic.Add("message", Message);

            return Yodo1JSON.Serialize(errorDic);
        }
    }
}