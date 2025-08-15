using ComplianceCrm.Domain.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplianceCrm.Application.Documents.Dtos;

/// <summary>DTO to update a document's status (e.g. Active, Archived, Deleted).</summary>
public sealed class UpdateDocumentStatusRequest
{
    public DocumentStatus Status { get; set; }
}