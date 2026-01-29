using RestSharp;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;


namespace Convertation
{
    internal class Program
    {


        static void Main(string[] args)
        {
            string ApiKey = "";
            try
            {
                if (File.Exists("APIKey.txt"))
                {
                    ApiKey = File.ReadAllText("APIKey.txt", Encoding.UTF8).Trim();
                }
                else
                {
                    Console.WriteLine("Файл APIKey.txt не найден!");
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка чтения файла APIKey.txt: {ex.Message}");
                return;
            }

            Convert("USD", "RUB", "25", ApiKey);

            static void Convert(string from, string to, string amount, string apiKey)
            {
                var client = new RestClient("https://api.apilayer.com");
                var request = new RestRequest($"currency_data/convert?from={from}&to={to}&amount={amount}", Method.Get);
                request.AddHeader("apikey", apiKey); // ← исправлено: apiKey вместо ApiKey
                RestResponse response = client.Execute(request);

                if (response.IsSuccessful)
                {
                    Console.WriteLine("Ответ API:");
                    Console.WriteLine(response.Content);

                    try
                    {
                        dynamic responseAnswer = JsonConvert.DeserializeObject<dynamic>(response.Content);

                        if (responseAnswer.success == true)
                        {
                            double result = responseAnswer.result;
                            Console.WriteLine($"\nРезультат: {amount} {from} = {result:F2} {to}");
                        }
                        else
                        {
                            Console.WriteLine($"Ошибка конвертации: {responseAnswer.error?.info ?? "Неизвестная ошибка"}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка парсинга JSON: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"Ошибка запроса: {response.StatusCode}");
                    Console.WriteLine(response.Content);
                }
            }

        }
    }
    }

