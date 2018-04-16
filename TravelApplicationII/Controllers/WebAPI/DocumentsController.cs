using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using TravelApplication.Class.Common;
using TravelApplication.Models;
using TravelApplication.Services;

namespace TravelApplication.Controllers.WebAPI
{
    public class DocumentsController : ApiController
    {
        IDocumentsService documentService = new DocumentsService();
        [HttpPost]
        [Route("api/documents/filesUpload")]
        public HttpResponseMessage Upload(int travelRequestId, int badgeNumber)
        {
            IDocumentsService documentsService = new DocumentsService();
            HttpResponseMessage msg = null;
            try
            {
                var request = HttpContext.Current.Request;
                HttpFileCollection allFiles = request.Files;
                HttpPostedFile uploadedFile = allFiles[0];
                FileInfo uploadedFileInfo = new FileInfo(uploadedFile.FileName);
                String extension = uploadedFileInfo.Extension;

                int imageLength = uploadedFile.ContentLength;
                string imageType = uploadedFile.ContentType;

                byte[] binaryImagedata = new byte[imageLength];
                uploadedFile.InputStream.Read(binaryImagedata, 0, imageLength);

                var endpointUrl = System.Configuration.ConfigurationManager.AppSettings["sharepointServiceUrl"].ToString() + "/SharePoint/UploadDocument";

                SharePointUpload uploadRequest = new SharePointUpload()
                {
                    documentStream = binaryImagedata, 
                    siteUrl = "http://mtaspw01/collaboration/InformationManagement/ATMS/apps",
                    documentListName = "TravelApp",
                    documentName = uploadedFile.FileName,
                    folder = badgeNumber.ToString() +"-"+ travelRequestId                   
                };

                documentsService.UploadToSharePoint(travelRequestId, uploadRequest);

                msg = Request.CreateResponse(HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                LogMessage.Log("api/documents/filesupload : " + ex.Message);
                msg = Request.CreateResponse(HttpStatusCode.InternalServerError, "Unable to upload document ");
            }
            return msg;

        }

        [HttpPost]
        [Route("api/documents/1/upload")]
        public HttpResponseMessage UploadDocument1(int travelRequestId, int badgeNumber)
        {
            IDocumentsService documentsService = new DocumentsService();
            HttpResponseMessage msg = null;
            try
            {
                var request = HttpContext.Current.Request;
                HttpFileCollection allFiles = request.Files;
                HttpPostedFile uploadedFile = allFiles[0];
                FileInfo uploadedFileInfo = new FileInfo(uploadedFile.FileName);
                String extension = uploadedFileInfo.Extension;

                int imageLength = uploadedFile.ContentLength;
                string imageType = uploadedFile.ContentType;

                byte[] binaryImagedata = new byte[imageLength];
                uploadedFile.InputStream.Read(binaryImagedata, 0, imageLength);

                var endpointUrl = System.Configuration.ConfigurationManager.AppSettings["sharepointServiceUrl"].ToString() + "/SharePoint/UploadDocument";

                SharePointUpload uploadRequest = new SharePointUpload()
                {
                    documentStream = binaryImagedata,
                    siteUrl = "http://mtaspw01/collaboration/InformationManagement/ATMS/apps",
                    documentListName = "TravelApp",
                    documentName = uploadedFile.FileName,
                    folder = badgeNumber.ToString() + "-" + travelRequestId
                };

                documentsService.UploadRequiredFileToSharePoint(travelRequestId, uploadRequest, 1);

                msg = Request.CreateResponse(HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                LogMessage.Log("api/documents/filesupload : " + ex.Message);
                msg = Request.CreateResponse(HttpStatusCode.InternalServerError, "Unable to upload document ");
            }
            return msg;

        }

        [HttpPost]
        [Route("api/documents/2/upload")]
        public HttpResponseMessage UploadDocument2(int travelRequestId, int badgeNumber)
        {
            IDocumentsService documentsService = new DocumentsService();
            HttpResponseMessage msg = null;
            try
            {
                var request = HttpContext.Current.Request;
                HttpFileCollection allFiles = request.Files;
                HttpPostedFile uploadedFile = allFiles[0];
                FileInfo uploadedFileInfo = new FileInfo(uploadedFile.FileName);
                String extension = uploadedFileInfo.Extension;

                int imageLength = uploadedFile.ContentLength;
                string imageType = uploadedFile.ContentType;

                byte[] binaryImagedata = new byte[imageLength];
                uploadedFile.InputStream.Read(binaryImagedata, 0, imageLength);

                var endpointUrl = System.Configuration.ConfigurationManager.AppSettings["sharepointServiceUrl"].ToString() + "/SharePoint/UploadDocument";

                SharePointUpload uploadRequest = new SharePointUpload()
                {
                    documentStream = binaryImagedata,
                    siteUrl = "http://mtaspw01/collaboration/InformationManagement/ATMS/apps",
                    documentListName = "TravelApp",
                    documentName = uploadedFile.FileName,
                    folder = badgeNumber.ToString() + "-" + travelRequestId
                };

                documentsService.UploadRequiredFileToSharePoint(travelRequestId, uploadRequest, 2);

                msg = Request.CreateResponse(HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                LogMessage.Log("api/documents/filesupload : " + ex.Message);
                msg = Request.CreateResponse(HttpStatusCode.InternalServerError, "Unable to upload document ");
            }
            return msg;

        }

        [HttpPost]
        [Route("api/documents/3/upload")]
        public HttpResponseMessage UploadDocument3(int travelRequestId, int badgeNumber)
        {
            IDocumentsService documentsService = new DocumentsService();
            HttpResponseMessage msg = null;
            try
            {
                var request = HttpContext.Current.Request;
                HttpFileCollection allFiles = request.Files;
                HttpPostedFile uploadedFile = allFiles[0];
                FileInfo uploadedFileInfo = new FileInfo(uploadedFile.FileName);
                String extension = uploadedFileInfo.Extension;

                int imageLength = uploadedFile.ContentLength;
                string imageType = uploadedFile.ContentType;

                byte[] binaryImagedata = new byte[imageLength];
                uploadedFile.InputStream.Read(binaryImagedata, 0, imageLength);

                var endpointUrl = System.Configuration.ConfigurationManager.AppSettings["sharepointServiceUrl"].ToString() + "/SharePoint/UploadDocument";

                SharePointUpload uploadRequest = new SharePointUpload()
                {
                    documentStream = binaryImagedata,
                    siteUrl = "http://mtaspw01/collaboration/InformationManagement/ATMS/apps",
                    documentListName = "TravelApp",
                    documentName = uploadedFile.FileName,
                    folder = badgeNumber.ToString() + "-" + travelRequestId
                };

                documentsService.UploadRequiredFileToSharePoint(travelRequestId, uploadRequest, 3);

                msg = Request.CreateResponse(HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                LogMessage.Log("api/documents/filesupload : " + ex.Message);
                msg = Request.CreateResponse(HttpStatusCode.InternalServerError, "Unable to upload document ");
            }
            return msg;

        }


        [HttpPost]
        [Route("api/documents/4/upload")]
        public HttpResponseMessage UploadDocument4(int travelRequestId, int badgeNumber)
        {
            IDocumentsService documentsService = new DocumentsService();
            HttpResponseMessage msg = null;
            try
            {
                var request = HttpContext.Current.Request;
                HttpFileCollection allFiles = request.Files;
                HttpPostedFile uploadedFile = allFiles[0];
                FileInfo uploadedFileInfo = new FileInfo(uploadedFile.FileName);
                String extension = uploadedFileInfo.Extension;

                int imageLength = uploadedFile.ContentLength;
                string imageType = uploadedFile.ContentType;

                byte[] binaryImagedata = new byte[imageLength];
                uploadedFile.InputStream.Read(binaryImagedata, 0, imageLength);

                var endpointUrl = System.Configuration.ConfigurationManager.AppSettings["sharepointServiceUrl"].ToString() + "/SharePoint/UploadDocument";

                SharePointUpload uploadRequest = new SharePointUpload()
                {
                    documentStream = binaryImagedata,
                    siteUrl = "http://mtaspw01/collaboration/InformationManagement/ATMS/apps",
                    documentListName = "TravelApp",
                    documentName = uploadedFile.FileName,
                    folder = badgeNumber.ToString() + "-" + travelRequestId
                };

                documentsService.UploadRequiredFileToSharePoint(travelRequestId, uploadRequest, 4);

                msg = Request.CreateResponse(HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                LogMessage.Log("api/documents/filesupload : " + ex.Message);
                msg = Request.CreateResponse(HttpStatusCode.InternalServerError, "Unable to upload document ");
            }
            return msg;

        }

        [HttpGet]
        [Route("api/documents/supportingdocuments")]
        public HttpResponseMessage SupportingDocuments(string travelRequestId, int badgeNumber)
        {
            HttpResponseMessage response = null;

            try
            {
                var supportingDocuments = documentService.GetAllDocumentsByTravelId(travelRequestId, badgeNumber);

                var data = new JavaScriptSerializer().Serialize(supportingDocuments.OrderByDescending(item => item.Id));

                response = Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                LogMessage.Log("api/documents/supportingdocuments : " + ex.Message);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return response;

        }

        [HttpGet]
        [Route("api/documents/requireddocuments")]
        public HttpResponseMessage RequiredDocuments(string travelRequestId, int badgeNumber)
        {
            HttpResponseMessage response = null;

            try
            {
                var requiredDocuments = documentService.GetAllRequiredDocumentsByTravelId(travelRequestId, badgeNumber);

                var data = new JavaScriptSerializer().Serialize(requiredDocuments.OrderByDescending(item => item.DocumentNumber));

                response = Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                LogMessage.Log("api/documents/requireddocuments : " + ex.Message);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return response;

        }
        [HttpDelete]
        [Route("api/documents/deletedocument")]
        public HttpResponseMessage DeleteDocument(int travelRequestId, int documentId)
        {
            HttpResponseMessage response = null;
            try
            {
                documentService.DeleteFilesByTravelId(travelRequestId, documentId);
                response = Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                LogMessage.Log("api/documents/deletedocument : " + ex.Message);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Document cannot be deleted.");
            }
            return response;
        }

        [HttpPost]
        [Route("api/documents/savenotes")]
        public HttpResponseMessage SaveTravelRequestNotes(JustifactionRequest justificationRequest)
        {
            HttpResponseMessage response = null;

            try
            {
                documentService.UploadDocumentNotes(justificationRequest.documentNoteRequest);
                response = Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                LogMessage.Log("api/documents/savenotes : " + ex.Message);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Notes cannot be saved.");
            }
            return response;
        }
    }


}