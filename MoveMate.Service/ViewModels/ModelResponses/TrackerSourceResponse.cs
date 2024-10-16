using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace MoveMate.Service.ViewModels.ModelResponses
{   
    [FirestoreData]
    public class TrackerSourceResponse
    {
        [FirestoreProperty]
        public int Id { get; set; }

        [FirestoreProperty]
        public int BookingTrackerId { get; set; }

        [FirestoreProperty]
        public string ResourceUrl { get; set; }

        public string ResourceCode { get; set; }

        [FirestoreProperty]
        public string Type { get; set; }
    }
}
