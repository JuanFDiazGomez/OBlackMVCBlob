using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace WebRole.Controllers {
	public class HomeController : Controller {

		BlobServices _blobServices = new BlobServices();

		public ActionResult Upload() {
			return View();
		}

		public ActionResult Index() {
			List<string> blobs = new List<string>();
			return View(_blobServices.GetContainerList);
		}

		[HttpPost]
		public ActionResult Upload(FormCollection Form) {
			string strContainer = Request.Form["container"];
			CloudBlobContainer blobContainer = _blobServices.GetCloudBlobContainer(strContainer.ToLower().Replace(" ",String.Empty));
			string uriSas = blobContainer.GetSharedAccessSignature(_blobServices.GetUploadSasConstraints);
			foreach (string item in Request.Files) {
				HttpPostedFileBase file = Request.Files[item] as HttpPostedFileBase;
				if (file.ContentLength > 0) {
					UploadFile(file, uriSas, blobContainer.Uri.ToString());
				}

			}
			return RedirectToAction("Index");
		}

		public ActionResult download(string pstrBlobName, string pstrContainerName) {
			CloudBlobContainer container = _blobServices.GetCloudBlobContainer(pstrContainerName);
			CloudBlockBlob blob = container.GetBlockBlobReference(pstrBlobName);
			string uriSas = blob.GetSharedAccessSignature(_blobServices.GetDownloadSasConstraints);
			return Redirect(blob.Uri + uriSas);
		}

		public void UploadFile(HttpPostedFileBase file, string pstrContainerSas, string pstrContrainerUri) {

			CloudBlockBlob blob = new CloudBlockBlob(new Uri($"{pstrContrainerUri}/{file.FileName}{pstrContainerSas}"));
			blob.Metadata["FileName"] = file.FileName;
			blob.Metadata["DateCreated"] = DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss");
			blob.Properties.ContentType = file.ContentType;
			blob.Properties.ContentDisposition = "attachment; filename = " + file.FileName;
			blob.UploadFromStream(file.InputStream);
		}
	}
}