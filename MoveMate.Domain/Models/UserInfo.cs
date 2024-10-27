using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class UserInfo
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? Type { get; set; }

    public string? ImgUrl { get; set; }

    public string? Code { get; set; }

    public string? Cavet { get; set; }

    public string? HealthInsurance { get; set; }

    public string? CitizenIdentification { get; set; }

    public string? HealthCertificate { get; set; }

    public string? License { get; set; }

    public string? PermanentAddress { get; set; }

    public string? TemporaryResidenceAddress { get; set; }

    public string? CurriculumVitae { get; set; }

    public bool? IsDeleted { get; set; }
    public virtual User? User { get; set; }
}
