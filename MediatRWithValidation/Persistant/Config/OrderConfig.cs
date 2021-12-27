using MediatRWithValidation.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediatRWithValidation.Persistant.Config
{
    public class OrderConfig : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x=>x.CustomerName).IsRequired().HasMaxLength(200);
            builder.Property(x=>x.CustomerAddress).HasMaxLength(200);
        }
    }
}
