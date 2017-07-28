using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
//using Microsoft.WindowsAzure.Storage.Auth;
//using Microsoft.WindowsAzure.Storage;
//using System.IO;
//using System.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRole {
	public class BlobServices {

		private CloudBlobClient blobClient;

		public CloudBlobContainer GetCloudBlobContainer(string psrtContainerName) {
			InitializeClient();
			CloudBlobContainer blobContainer = blobClient.GetContainerReference(psrtContainerName);
			if (blobContainer.CreateIfNotExists()) {
				blobContainer.SetPermissions(new BlobContainerPermissions {
					PublicAccess = BlobContainerPublicAccessType.Blob
				});
			}
			return blobContainer;
		}

		public SharedAccessBlobPolicy GetUploadSasConstraints {
			get {
				SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy();
				sasConstraints.SharedAccessStartTime = DateTime.Now.AddMinutes(-15);
				sasConstraints.SharedAccessExpiryTime = DateTime.Now.AddMinutes(20);
				sasConstraints.Permissions = SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.Create;
				return sasConstraints;
			}
		}

		public SharedAccessBlobPolicy GetDownloadSasConstraints {
			get {
				SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy();
				sasConstraints.SharedAccessStartTime = DateTime.Now.AddMinutes(-15);
				sasConstraints.SharedAccessExpiryTime = DateTime.Now.AddMinutes(20);
				sasConstraints.Permissions = SharedAccessBlobPermissions.Read;
				return sasConstraints;
			}
		}

		public IEnumerable<CloudBlobContainer> GetContainerList {
			get {
				InitializeClient();
				return blobClient.ListContainers();
			}
		}

		private void InitializeClient() {
			string strConnection = "DefaultEndpointsProtocol=https;AccountName=dublinoback;AccountKey=SFQUMXVQnfi/rRIL9HU6Jo5VJ2BDkg+NXvLok0nFD1PI92XS9o59Y1cqcp86qToj2c3yz3J+3CTRKxiVqIS0GA==;EndpointSuffix=core.windows.net";
			CloudStorageAccount storageAccount = CloudStorageAccount.Parse(strConnection);
			blobClient = storageAccount.CreateCloudBlobClient();
		}
	}
}