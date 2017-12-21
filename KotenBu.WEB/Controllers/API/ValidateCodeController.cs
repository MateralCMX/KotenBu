using MateralTools.MCache;
using MateralTools.MEncryption;
using MateralTools.MVerify;
using MateralTools.MVerify.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace KotenBu.WEB.Controllers.API
{
    /// <summary>
    /// 验证码控制器
    /// </summary>
    [RoutePrefix("api/ValidateCode")]
    [NotVerificationLogin]
    public class ValidateCodeController : ApiBaseController
    {
        /// <summary>
        /// 获得验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetValidateCode")]
        public HttpResponseMessage GetValidateCode()
        {
            VerifyCodeManager vcMa = new VerifyCodeManager();
            /*背景是否为图片还是纯色*/
            vcMa.TextConfigM.BackIsImage = true;
            /*背景图片路径(BackIsImage==true时生效)*/
            vcMa.TextConfigM.ImageBackgroundPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\Images\ValidateCode";
            /*背景颜色库(BackIsImage==false时生效)*/
            vcMa.TextConfigM.BackgroundColors.Add(Color.Ivory);
            /*值颜色库*/
            vcMa.TextConfigM.ValueColors.Add(Color.Red);
            /*文本库(AllowRandomChinese==true时失效)*/
            vcMa.TextConfigM.TextLibrary = new List<string>
            {
                //"abcdefghijklmnopqrstuvwxyz",
                "ABCDEFGHIJKLMNOPQRSTUVWXYZ",
                "0123456789"
            };
            //vcMa.TextConfigM.TextLibrary.Add("陈明旭中华人民共和国");
            /*采用随机中文模式*/
            vcMa.TextConfigM.AllowRandomChinese = false;
            /*值数量*/
            vcMa.TextConfigM.ValueCount = 4;
            /*混淆数量*/
            vcMa.TextConfigM.ConfusionCount = 10;
            /*混淆模式*/
            vcMa.TextConfigM.ImageObfuscationTypes = new List<VerifyCodeImageObfuscationType>
            {
                /*假值混淆*/
                VerifyCodeImageObfuscationType.FalseValue,
                /*条纹混淆*/
                VerifyCodeImageObfuscationType.Stripe
            };
            /*图片大小*/
            vcMa.TextConfigM.ImageSize = new Size(120, 30);
            /*字体大小*/
            vcMa.TextConfigM.FontSize = 18;
            /*获取验证码*/
            VerifyCodeModel vcM = vcMa.GetVeifyCodeModel();
            if (vcM.Images.Count > 0)
            {
                /*验证码的值存入缓存中*/
                WebCacheManager.Set(ApplicationManager.VALIDATECODEKEY + vcM.Value, vcM.Value, DateTimeOffset.Now.AddMinutes(5));
                using (MemoryStream ms = new MemoryStream())
                {
                    vcM.Images[0].Save(ms, ImageFormat.Jpeg);
                    HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(ms.ToArray())
                    };
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                    return result;
                }
            }
            return null;
        }

        ///// <summary>
        ///// 获得手机验证码
        ///// </summary>
        ///// <param name="queryM">查询模型</param>
        ///// <returns>职务信息</returns>
        //[HttpPost]
        //[Route("GetPhoneCode")]
        //public MResultModel<string> GetPhoneCode(PhoneCodeModel queryM)
        //{
        //    if (VerificationImageCode(queryM.ValidateCode))
        //    {
        //        Random rd = new Random();
        //        string phoneCode = rd.Next(1000, 9999).ToString();
        //        int EffectiveTime = 15;
        //        string smsContent = string.Format("您本次验证码为{0}，{1}分钟内有效。", phoneCode, EffectiveTime);
        //        if (SMSManager.SendSms(queryM.Mobile, smsContent))
        //        {
        //            WebCacheManager.Set(ApplicationManager.PHONECODEKEY + queryM.Mobile, phoneCode, DateTimeOffset.Now.AddMinutes(EffectiveTime));
        //            return MResultModel<string>.GetSuccessResultM(phoneCode, "获取成功");
        //        }
        //        else
        //        {
        //            return MResultModel<string>.GetFailResultM(null, "获取失败，短信发送失败");
        //        }
        //    }
        //    else
        //    {
        //        return MResultModel<string>.GetFailResultM(null, "获取失败，图形验证码错误");
        //    }

        //}
        ///// <summary>
        /////  获取二维码
        ///// </summary>
        ///// <param name="OrderID">订单ID</param>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("GetQRImage")]
        //public HttpResponseMessage GetQRImage(Guid OrderID)
        //{
        //    T_Order orderM = new OrderManager().GetOrderInfoByID(OrderID).Data;
        //    HttpResponseMessage result;
        //    if (orderM != null)
        //    {
        //        T_BluePrint bluePrintM = orderM.T_BluePrint.FirstOrDefault();
        //        T_Decoration decorationM = orderM.T_Decoration.FirstOrDefault();
        //        object obj = new
        //        {
        //            BluePrintID = bluePrintM?.ID.ToString(),
        //            DecorationID = decorationM?.ID.ToString(),

        //        };
        //        string json = MateralTools.MConvert.ConvertManager.ModelToJson(obj);
        //        Bitmap img = EncryptionManager.QRCodeEncode(json, 0);
        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            img.Save(ms, ImageFormat.Jpeg);
        //            img.Dispose();
        //            result = new HttpResponseMessage(HttpStatusCode.OK)
        //            {
        //                Content = new ByteArrayContent(ms.ToArray())
        //            };
        //            result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        //        }
        //    }
        //    else
        //    {
        //        result = new HttpResponseMessage(HttpStatusCode.BadRequest);
        //    }
        //    return result;
        //}
    }
}