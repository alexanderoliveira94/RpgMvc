using Microsoft.AspNetCore.Mvc;
using CamisasMvc.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.IO;



namespace CamisasMvc.Controllers
{
    public class CamisasController : Controller
    {
        public string uriBase = "http://AlexanderOliveira.somee.com/CamisasApi/Camisas/";




        [HttpGet]
        public async Task<ActionResult> IndexAsync()
        {
            try
            {
                string uriComplementar = "GetAll";
                HttpClient httpClient = new HttpClient();
                string token = HttpContext.Session.GetString("SessionTokenUsuario");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await httpClient.GetAsync(uriBase + uriComplementar);
                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    List<CamisasViewModel> listaCamisas = await Task.Run(() =>
                    JsonConvert.DeserializeObject<List<CamisasViewModel>>(serialized));

                    return View(listaCamisas);
                }
                else
                    throw new System.Exception(serialized);
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(CamisasViewModel p)
        {
            try
            {
                if (p.Valor <= 0)
                {
                    TempData["MensagemErro"] = "O valor da camisa deve ser maior que 0.";
                    return RedirectToAction("Create");

                }

                HttpClient httpClient = new HttpClient();
                string token = HttpContext.Session.GetString("SessionTokenUsuario");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var content = new StringContent(JsonConvert.SerializeObject(p));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = await httpClient.PostAsync(uriBase, content);
                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    TempData["Mensagem"] = string.Format("Camisa do {0}, Id{1} salvo com sucesso!", p.Nome, serialized);
                    return RedirectToAction("Index");
                }
                else
                    throw new System.Exception(serialized);
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Create");
            }
        }
        [HttpGet]
        public ActionResult Create()
        {
            CamisasViewModel model = new CamisasViewModel();


            model.ListaClasses = new List<string> { "Perfeita", "Boa", "Malhada" };

            return View(model);
        }


        [HttpGet]
        public async Task<ActionResult> DetailsAsync(int? id)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string token = HttpContext.Session.GetString("SessionTokenUsuario");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await httpClient.GetAsync(uriBase + id.ToString());
                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    CamisasViewModel p = await Task.Run(() =>
                    JsonConvert.DeserializeObject<CamisasViewModel>(serialized));
                    return View(p);
                }
                else
                    throw new System.Exception(serialized);
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Index");
            }

        }

        [HttpGet]
        public async Task<ActionResult> EditAsync(int? id)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string token = HttpContext.Session.GetString("SessionTokenUsuario");

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.GetAsync(uriBase + id.ToString());

                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    CamisasViewModel p = await Task.Run(() =>
                    JsonConvert.DeserializeObject<CamisasViewModel>(serialized));

                    // Preencher a lista de classes disponíveis
                    p.ListaClasses = new List<string> { "Perfeita", "Boa", "Malhada" };

                    return View(p);
                }
                else
                {
                    throw new System.Exception(serialized);
                }
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<ActionResult> EditAsync(CamisasViewModel p)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string token = HttpContext.Session.GetString("SessionTokenUsuario");

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var content = new StringContent(JsonConvert.SerializeObject(p));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = await httpClient.PutAsync(uriBase, content);
                string serialized = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    TempData["Mensagem"] =
                    string.Format("Camisa {0}, classe {1} atualizado com sucesso", p.Nome, p.Classe);

                    return RedirectToAction("Index");
                }
                else
                    throw new System.Exception(serialized);
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string token = HttpContext.Session.GetString("SessionTokenUsuario");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await httpClient.DeleteAsync(uriBase + id.ToString());
                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    TempData["Mensagem"] = string.Format("Camisa Id {0} removido com sucesso!", id);
                    return RedirectToAction("Index");
                }
                else
                    throw new Exception(serialized);
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public ActionResult AdicionarFoto(int id)
        {
            var model = new AdicionarFotoViewModel
            {
                CamisaId = id
            };

            return View(model);
        }


        [HttpPost]
        public async Task<ActionResult> AdicionarFoto(int id, IFormFile foto)
        {
            try
            {
                if (foto == null || foto.Length <= 0)
                {
                    TempData["MensagemErro"] = "Nenhuma foto selecionada.";
                    return RedirectToAction("Edit", new { id });
                }

                HttpClient httpClient = new HttpClient();
                string token = HttpContext.Session.GetString("SessionTokenUsuario");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                using (var content = new MultipartFormDataContent())
                {
                    content.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

                    // Adicionar o arquivo da foto à requisição
                    using (var streamContent = new StreamContent(foto.OpenReadStream()))
                    {
                        content.Add(streamContent, "foto", foto.FileName);

                        HttpResponseMessage response = await httpClient.PostAsync(uriBase + id + "/AdicionarFoto", content);
                        string serialized = await response.Content.ReadAsStringAsync();

                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            TempData["Mensagem"] = "Foto adicionada com sucesso!";
                            return RedirectToAction("Edit", new { id });
                        }
                        else
                        {
                            throw new System.Exception(serialized);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Edit", new { id });
            }
        }




    }
}

