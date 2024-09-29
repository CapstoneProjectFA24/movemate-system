using MoveMate.Service.ThirdPartyService.Momo.Config;
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.ViewModels.ModelResponses;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MoveMate.Service.IServices;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.Services;

namespace MoveMate.Service.ThirdPartyService.Momo
{
    public class MomoService : IMomoService
    {
        
            private readonly UnitOfWork _unitOfWork;
            private readonly IMapper _mapper;
            private readonly ILogger<MomoService> _logger;

            public MomoService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<MomoService> logger)
            {
                _unitOfWork = (UnitOfWork)unitOfWork;
                _mapper = mapper;
                _logger = logger;
            }

            public async Task<(bool success, string? payUrl)> CreatePaymentLink(
                string requestId,
                string orderId,
                string amount,
                string orderInfo,
                string extraData = "",
                string lang = "vi")
            {
                // Create payment request
                var paymentRequest = new MomoOneTimePaymentRequest(
                    partnerCode: MomoConfig.PartnerCode,  // Static field from MomoConfig
                    requestId: requestId,
                    amount: amount,
                    orderId: orderId,
                    orderInfo: orderInfo,
                    redirectUrl: MomoConfig.ReturnUrl,  // Static field from MomoConfig
                    ipnUrl: MomoConfig.IpnUrl,  // Static field from MomoConfig
                    requestType: "captureWallet",
                    extraData: extraData,
                    lang: lang
                );

                // Generate signature
                paymentRequest.MakeSignature(MomoConfig.AccessKey, MomoConfig.SceretKey);  // Ensure correct secret key is used

                // Serialize request to JSON
                var requestData = JsonConvert.SerializeObject(paymentRequest);
                var requestContent = new StringContent(requestData, Encoding.UTF8, "application/json");

                // Send request to MoMo API
                using var httpClient = new HttpClient();

                // Make sure the PaymentUrl is a valid, absolute URL
                if (!Uri.IsWellFormedUriString(MomoConfig.PaymentUrl, UriKind.Absolute))
                {
                    _logger.LogError($"Invalid MoMo Payment URL: {MomoConfig.PaymentUrl}");
                    return (false, "Invalid payment URL.");
                }

                var response = await httpClient.PostAsync(MomoConfig.PaymentUrl, requestContent);  // Make sure this is an absolute URL

                // Handle response
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseData = JsonConvert.DeserializeObject<MomoOneTimePaymentResponse>(responseContent);

                    if (responseData?.resultCode == "0")
                    {
                        return (true, responseData.payUrl);
                    }
                    else
                    {
                        _logger.LogError($"MoMo API Error: {responseData?.message}");
                        return (false, responseData?.message);
                    }
                }
                else
                {
                    _logger.LogError($"HTTP Error: {response.StatusCode} - {response.ReasonPhrase}");
                    return (false, response.ReasonPhrase);
                }
            }
        }
    
    }

