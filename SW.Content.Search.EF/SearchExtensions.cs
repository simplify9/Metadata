using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Search.EF
{
    public static class SearchExtensions
    {
        public static void AddContentSearchIndex(this ModelBuilder b, string schemaName = "search")
        {
            var doc = b.Entity<DbDoc>();
            doc.ToTable("Docs", schemaName);
            doc.HasKey(d => d.Id);
            doc.Property(d => d.SourceIdString).IsUnicode(false).HasMaxLength(128);
            doc.Property(d => d.SourceType).IsUnicode(false).HasMaxLength(256);
            doc.HasMany(d => d.Tokens).WithOne(t => t.Document);
            doc.HasIndex(d => d.SourceIdString);
            doc.HasIndex(d => d.SourceType);

            var token = b.Entity<DbDocToken>();
            token.ToTable("DocTokens", schemaName);
            token.HasKey(t => t.Id);
            token.HasOne(t => t.Path).WithMany();
           // token.Property(t => t.ValueAsString).IsUnicode(true).HasMaxLength(512);
            token.Property(t => t.ValueAsAny).IsUnicode(true).HasMaxLength(512);
            token.HasIndex(new string[] { "PathId", "ValueAsAny" });
            token.HasIndex(t=>t.ValueAsAny);

            //token.Property(t => t.ValueAsNumber).HasColumnType("DECIMAL(19,6)");

            var path = b.Entity<DbDocSourcePath>();
            path.ToTable("DocPaths", schemaName);
            path.HasKey(p => p.Id);
            path.Property(p => p.DocumentType).IsUnicode(false).HasMaxLength(256);
            path.Property(p => p.PathString).IsUnicode(false).HasMaxLength(256);

        }
    }
}
