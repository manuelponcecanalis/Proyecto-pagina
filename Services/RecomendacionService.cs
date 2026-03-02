using Pagina_proyecto.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text;
using Pagina_proyecto.Models.ViewModels;


namespace Pagina_proyecto.Areas.Data

{
    public class RecomendacionService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<RecomendacionService> _logger;

        public RecomendacionService(AppDbContext context, ILogger<RecomendacionService> logger)
        {
            _context = context;
            _logger = logger;
        }



        private object FormatearRecomendacion(OfertaCalificada r)
        {
            return new
            {
                ID = r.ID,
                Materia = r.Materia ?? "Sin datos",
                Docente = r.Docente ?? "Sin datos",
                Sede = r.Sede ?? "Sin datos",
                DiasDeCursada = r.DiasDeCursada ?? "Sin datos",
                HorarioDeCursada = r.HorarioDeCursada ?? "Sin datos",
                Cuatrimestre = r.Cuatrimestre ?? "Sin datos",
                ModalidadDeCursada = r.ModalidadDeCursada ?? "Sin datos",
                Puntaje = r.Puntaje ?? "Sin datos",
                NivelDeLaCursada = r.NivelDeLaCursada ?? "Sin datos",
                DificultadDelCurso = r.DificultadDelCurso ?? "Sin datos",
                CalidadDeLasClases = r.CalidadDeLasClases ?? "Sin datos",
                FormatoDeLasClases = r.FormatoDeLasClases ?? "Sin datos",
                SeRespetoElDiaVirtualAsignado = r.SeRespetoElDiaVirtualAsignado ?? "Sin datos",
                ModalidadDeLosParciales = r.ModalidadDeLosParciales ?? "Sin datos",
                DificultadDeLosParciales = r.DificultadDeLosParciales ?? "Sin datos",
                RecomendariasElCurso = r.RecomendariasElCurso ?? "Sin datos",
                OtraInfo = r.OtraInfo ?? "Sin datos"
            };
        }


        public async Task<List<object>> ObtenerTodasAsync()
        {
            try
            {
                var recomendaciones = await _context.RecomendacionesFCE
                    .AsNoTracking()
                    .OrderBy(r => r.ID)
                    .ToListAsync();

                var resultado = recomendaciones.Select(FormatearRecomendacion).ToList();

                _logger.LogInformation($"Se obtuvieron {resultado.Count} RecomendacionesFCE");
                return resultado.Cast<object>().ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las Recomendaciones FCE");
                throw;
            }
        }

        public List<object> ObtenerTodas()
        {
            try
            {
                var recomendaciones = _context.RecomendacionesFCE
                    .AsNoTracking()
                    .OrderBy(r => r.Materia == null)
                    .ThenBy(r => r.Materia)
                    .ToList();

                var resultado = recomendaciones.Select(FormatearRecomendacion).ToList();

                _logger.LogInformation($"Se obtuvieron {resultado.Count} Recomendaciones FCE");
                return resultado.Cast<object>().ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las Recomendaciones FCE");
                throw;
            }
        }

        public async Task<OfertaCalificada> CrearAsync(OfertaCalificada recomendacion)
        {
            try
            {
                var maxId = await _context.RecomendacionesFCE.AnyAsync()
                    ? await _context.RecomendacionesFCE.MaxAsync(r => r.ID)
                    : 0;

                recomendacion.ID = (short)(maxId + 1);

                // Validar que no exceda el límite de short
                if (recomendacion.ID < 0)
                    throw new OverflowException("El ID excede el valor mínimo permitido para short.");
                if (recomendacion.ID > short.MaxValue)
                    throw new OverflowException("El ID excede el valor máximo permitido para short.");

                _context.RecomendacionesFCE.Add(recomendacion);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Recomendación FCE creada con ID: {recomendacion.ID}");
                return recomendacion;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la recomendación FCE");
                throw;
            }
        }

        public async Task<OfertaCalificada> ObtenerPorIdAsync(int id)
        {
            try
            {
                var recomendacion = await _context.RecomendacionesFCE.FindAsync(id);
                if (recomendacion == null)
                {
                    _logger.LogWarning($"No se encontró la recomendación FCE con ID: {id}");
                    return null;
                }
                return recomendacion;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener la recomendación FCE con ID: {id}");
                throw;
            }
        }

        public async Task<OfertaCalificada> ActualizarAsync(OfertaCalificada recomendacion)
        {
            try
            {
                _context.Entry(recomendacion).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Recomendación FCE actualizada con ID: {recomendacion.ID}");
                return recomendacion;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar la recomendación FCE con ID: {recomendacion.ID}");
                throw;
            }
        }

        public async Task<bool> EliminarAsync(int id)
        {
            try
            {
                var recomendacion = await _context.RecomendacionesFCE.FindAsync(id);
                if (recomendacion == null)
                {
                    _logger.LogWarning($"No se encontró la recomendación FCE con ID: {id}");
                    return false;
                }

                _context.RecomendacionesFCE.Remove(recomendacion);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Recomendación FCE eliminada con ID: {id}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar la recomendación FCE con ID: {id}");
                return false;
            }
        }

        public async Task<List<object>> FiltrarAsync(string filtro)
        {
            try
            {
                filtro = filtro?.ToLower() ?? "";

                var recomendaciones = await _context.RecomendacionesFCE
                    .AsNoTracking()
                    .Where(r =>
                        (r.Materia != null && r.Materia.ToLower().Contains(filtro)) ||
                        (r.Docente != null && r.Docente.ToLower().Contains(filtro)) ||
                        (r.Sede != null && r.Sede.ToLower().Contains(filtro)) ||
                        (r.HorarioDeCursada != null && r.HorarioDeCursada.ToLower().Contains(filtro)) ||
                        (r.DiasDeCursada != null && r.DiasDeCursada.ToLower().Contains(filtro)))
                    .OrderBy(r => r.ID)
                    .ToListAsync();

                var resultado = recomendaciones.Select(FormatearRecomendacion).ToList();

                _logger.LogInformation($"Se obtuvieron {resultado.Count} Recomendaciones FCE filtradas");
                return resultado.Cast<object>().ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al filtrar Recomendaciones FCE con filtro: {filtro}");
                throw;
            }
        }

        public async Task<List<object>> FiltrarPorMateriaAsync(string materia)
        {
            try
            {
                var recomendaciones = await _context.RecomendacionesFCE
                    .AsNoTracking()
                    .Where(r => r.Materia != null && r.Materia.Contains(materia))
                    .OrderBy(r => r.ID)
                    .ToListAsync();

                var resultado = recomendaciones.Select(FormatearRecomendacion).ToList();

                _logger.LogInformation($"Se obtuvieron {resultado.Count} Recomendaciones FCE filtradas por materia: {materia}");
                return resultado.Cast<object>().ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al filtrar Recomendaciones FCE por materia: {materia}");
                throw;
            }
        }

        public async Task<int> ContarTotalAsync()
        {
            try
            {
                return await _context.RecomendacionesFCE.AsNoTracking().CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al contar las Recomendaciones FCE");
                throw;
            }
        }

        public async Task<List<string>> ObtenerMateriasDistintasAsync()
        {
            try
            {
                return await _context.RecomendacionesFCE
                    .AsNoTracking()
                    .Where(r => !string.IsNullOrWhiteSpace(r.Materia))
                    .Select(r => r.Materia)
                    .Distinct()
                    .OrderBy(m => m)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener materias distintas");
                throw;
            }
        }

        public async Task<List<string>> ObtenerDocentesDistintosAsync()
        {
            try
            {
                return await _context.RecomendacionesFCE
                    .AsNoTracking()
                    .Where(r => !string.IsNullOrWhiteSpace(r.Docente))
                    .Select(r => r.Docente)
                    .Distinct()
                    .OrderBy(d => d)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener docentes distintos");
                throw;
            }
        }

        public async Task<bool> ImportarDatosMasivamenteAsync(List<OfertaCalificada> recomendaciones)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var maxId = await _context.RecomendacionesFCE.AnyAsync()
                    ? await _context.RecomendacionesFCE.MaxAsync(r => r.ID)
                    : 0;

                foreach (var recom in recomendaciones)
                {
                    recom.ID = (short)(maxId + 1);
                    maxId++;

                    // Validar código duplicado
                    if (await _context.RecomendacionesFCE.AnyAsync(r => r.ID == recom.ID))
                        throw new Exception($"Ya existe una recomendación con ID: {recom.ID}");

                    _context.RecomendacionesFCE.Add(recom);
                }

                var resultado = await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation($"Se importaron {resultado} Recomendaciones FCE exitosamente");
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al importar datos masivamente");
                return false;
            }
        }
    }
}
