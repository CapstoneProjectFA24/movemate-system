using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelResponses
{
    [FirestoreData]
    public class NotificationResponse
    {
        [FirestoreProperty] public int Id { get; set; }

        [FirestoreProperty] public int UserId { get; set; }

        [FirestoreProperty] public string SentFrom { get; set; }

        [FirestoreProperty] public string Receive { get; set; }

        [FirestoreProperty] public string Name { get; set; }

        [FirestoreProperty] public string Description { get; set; }

        [FirestoreProperty] public string Topic { get; set; }

        [FirestoreProperty] public bool IsRead { get; set; }

    }
}
