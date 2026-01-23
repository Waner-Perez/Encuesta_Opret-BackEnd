using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using WebApiForm.DTO__Data_Transfer_Object_;
using WebApiForm.Repository.Models;
using WebApiForm.Services.DTO__Data_Transfer_Object_;
using WebApiForm.Services;

namespace WebApiForm.Repository;

public partial class FormEncuestaDbContext : DbContext
{
    public FormEncuestaDbContext()
    {
    }

    public FormEncuestaDbContext(DbContextOptions<FormEncuestaDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Estacion> Estacions { get; set; }

    public virtual DbSet<Formulario> Formularios { get; set; }

    public virtual DbSet<Linea> Lineas { get; set; }

    public virtual DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

    public virtual DbSet<Pregunta> Preguntas { get; set; }

    public virtual DbSet<RegistroUsuario> RegistroUsuarios { get; set; }

    public virtual DbSet<Respuesta> Respuestas { get; set; }

    public virtual DbSet<Sesion> Sesions { get; set; }

    public virtual DbSet<SubPregunta> SubPreguntas { get; set; }

    // DbSet para DTOs (entidades sin clave primaria)
    public DbSet<PreguntaCompleta> PreguntaCompletas { get; set; }
    public DbSet<EstacionPorLinea> EstacionPorLineas { get; set; }
    public DbSet<ObtenerForm_Dto> obtenerFormDtos { get; set; }
    public DbSet<ObtenerRespuestas_Dto> obtenerRespuestasDtos { get; set; }
    public DbSet<ReportRespuestas_Dto> reportRespuestas_Dtos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Estacion>(entity =>
        {
            entity.HasKey(e => e.IdEstacion).HasName("PK__Estacion__1F3B45EB0B6C2E44");

            entity.Property(e => e.IdEstacion).ValueGeneratedNever();

            entity.HasOne(d => d.IdLineaNavigation).WithMany(p => p.Estacions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Estacion_linea");
        });

        modelBuilder.Entity<Formulario>(entity =>
        {
            entity.HasKey(e => e.IdentifacadorForm).HasName("PK__Formular__6CDA1CA2B18434A8");

            entity.HasOne(d => d.IdEstacionNavigation).WithMany(p => p.Formularios).HasConstraintName("fk_Formulario_Estacion");

            entity.HasOne(d => d.IdLineaNavigation).WithMany(p => p.Formularios).HasConstraintName("fk_Formulario_Linea");

            entity.HasOne(d => d.IdUsuariosNavigation).WithMany(p => p.Formularios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_User_Form");
        });

        modelBuilder.Entity<Linea>(entity =>
        {
            entity.HasKey(e => e.IdLinea).HasName("PK__Linea__E346BA199780FE6E");
        });

        modelBuilder.Entity<PasswordResetToken>(entity =>
        {
            entity.HasKey(e => e.Token).HasName("PK__Password__CA90DA7BCC5754EE");

            entity.HasOne(d => d.IdUsuariosNavigation).WithMany(p => p.PasswordResetTokens)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Token_Usuarios");
        });

        modelBuilder.Entity<Pregunta>(entity =>
        {
            entity.HasKey(e => e.CodPregunta).HasName("PK__Pregunta__9277FCFE6ABE70E8");

            entity.Property(e => e.CodPregunta).ValueGeneratedNever();
        });

        modelBuilder.Entity<RegistroUsuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuarios).HasName("PK__Registro__854B73B3C110A7C4");

            entity.ToTable(tb => tb.HasTrigger("trg_Increment_Usuarios"));

            entity.Property(e => e.FechaCreacion)
                  .HasColumnName("fecha_creacion")
                  .HasDefaultValueSql("GETDATE()")
                  .ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Respuesta>(entity =>
        {
            entity.HasKey(e => e.IdRespuestas).HasName("PK__Respuest__D875135C29C85BCD");

            entity.HasOne(d => d.IdSesionNavigation).WithMany(p => p.Respuestas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Respuestas_Sesion");

            entity.HasOne(d => d.IdUsuariosNavigation).WithMany(p => p.Respuestas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Respuestas_User");

            entity.HasOne(d => d.IdentifacadorFormNavigation).WithMany(p => p.Respuestas).HasConstraintName("fk_Respuestas_Form");
        });

        modelBuilder.Entity<Sesion>(entity =>
        {
            entity.HasKey(e => e.IdSesion).HasName("PK__Sesion__8D3F9DFED1B0301D");

            entity.ToTable("Sesion", tb => tb.HasTrigger("trg_increment_Sesion"));

            entity.Property(e => e.IdSesion).ValueGeneratedNever();

            entity.HasOne(d => d.CodPreguntaNavigation).WithMany(p => p.Sesions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Sesion_Pregunta");

            entity.HasOne(d => d.CodSubPreguntaNavigation).WithMany(p => p.Sesions).HasConstraintName("fk_Sesion_SubPreguntas");
        });

        modelBuilder.Entity<SubPregunta>(entity =>
        {
            entity.HasKey(e => e.CodSubPregunta).HasName("PK__SubPregu__B4EDE11CDD6397F2");
        });

        modelBuilder.Entity<PreguntaCompleta>().HasNoKey();
        modelBuilder.Entity<EstacionPorLinea>().HasNoKey();
        modelBuilder.Entity<ObtenerForm_Dto>().HasNoKey();
        modelBuilder.Entity<ObtenerRespuestas_Dto>().HasNoKey();
        modelBuilder.Entity<ReportRespuestas_Dto>().HasNoKey();

        base.OnModelCreating(modelBuilder);
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
