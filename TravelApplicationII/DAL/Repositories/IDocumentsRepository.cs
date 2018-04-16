﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelApplication.Models;

namespace TravelApplication.DAL.Repositories
{
    public interface IDocumentsRepository
    {
        void UploadFileInfo(int travelRequestId, string fileName);

        void UploadRequiredFileInfo(int travelRequestId, string fileName, int requiredFileOrder);
        List<SupportingDocument> GetAllDocumentsByTravelId(string travelRequestId, int badgeNumber);
        List<RequiredDocuments> GetAllRequiredDocumentsByTravelId(string travelRequestId, int badgeNumber);

        void DeleteFilesByTravelId(int travelRequestId, int id);
        void UploadDocumentNotes(DocumentNoteRequest documentNoteRequest);
    }
}