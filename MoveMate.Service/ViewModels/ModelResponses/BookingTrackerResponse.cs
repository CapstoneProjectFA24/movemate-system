using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace MoveMate.Service.ViewModels.ModelResponses
{
    [FirestoreData]
    public class BookingTrackerResponse
    {
        [FirestoreProperty] public int Id { get; set; }
        [FirestoreProperty] public int BookingId { get; set; }
        [FirestoreProperty] public string Time { get; set; }
        [FirestoreProperty] public string Type { get; set; }

        public string Location { get; set; }

        public string Point { get; set; }
        [FirestoreProperty] public string Description { get; set; }
        [FirestoreProperty] public List<TrackerSourceResponse> TrackerSources { get; set; }
    }
}