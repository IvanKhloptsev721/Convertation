using RestSharp;
using Newtonsoft.Json;


namespace Convertation
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Convert("USD", "RUB", "5");

            static void Convert(string from, string to, string amount) // Исправил порядок параметров
            {
                var client = new RestClient("https://api.apilayer.com");

                // ОШИБКА: в URL пропущено = после from. И порядок параметров неправильный
                // Правильно: from={from}&to={to}
                var request = new RestRequest($"currency_data/convert?from={from}&to={to}&amount={amount}", Method.Get);
                request.AddHeader("apikey", "lOmNJehjGlzCBAIkgabfM09sG9PpsPVH");

                // ВАЖНО: Dispose response после использования
                RestResponse response = client.Execute(request); // ExecuteGet устарел

                if (response.IsSuccessful)
                {
                    Console.WriteLine("Ответ API:");
                    Console.WriteLine(response.Content);

                    // Десериализация
                    try
                    {
                        dynamic responseAnswer = JsonConvert.DeserializeObject<dynamic>(response.Content);

                        // Проверяем успешность конвертации
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