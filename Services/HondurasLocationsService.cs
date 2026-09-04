using System.Collections.Generic;

namespace SMART_ERP.Services;

public static class HondurasLocationsService
{
    public static Dictionary<string, List<string>> GetDepartmentsAndCities()
    {
        return new Dictionary<string, List<string>>
        {
            { "Atlántida", new List<string> { "La Ceiba", "Tela", "Tocoa", "Jutiapa", "Arizona", "Esparta", "Limón", "San Francisco", "Masica", "Santa Rosa de Aguán", "Saba", "Santa Bárbara", "Iriona", "Balfate" } },
            { "Colón", new List<string> { "Trujillo", "Tocoa", "Bonito Oriental", "Balfate", "Irusmo", "Santa Fe", "Saba", "Santa Rosa de Aguán", "Santa Bárbara", "Iriona", "Jutiapa", "Limón", "Sonaguera", "Santa Rosa", "San Esteban", "Vallecito" } },
            { "Comayagua", new List<string> { "Comayagua", "Támara", "La Palmira", "Siguatepeque", "Las Lajas", "San Jerónimo", "Taulabé", "La Trinidad", "El Rosario", "Lejamani", "Meámbar", "San José de Comayagua", "Valle de Ángeles", "Humuya", "San Sebastián", "Lamaní", "Ajuterique" } },
            { "Copán", new List<string> { "Santa Rosa de Copán", "Copán Ruinas", "La Entrada", "Florida", "San Agustín", "San Antonio", "San Jerónimo", "San José", "San Nicolás", "San Pedro", "Santa Cruz", "Santiago", "Cabañas", "El Paraíso", "El Florido", "Río Amarillo", "Concepción", "Veracruz", "Corquín", "Florida Opalaca", "La Jigua", "San Juan de Opoa" } },
            { "Cortés", new List<string> { "San Pedro Sula", "Choloma", "Omoa", "Puerto Cortés", "La Lima", "Villanueva", "Tela", "El Progreso", "La Ceiba", "La Masica", "Tocoa", "Arizona", "Esparta", "Saba", "Santa Rosa de Aguán", "Santa Bárbara", "Iriona", "Balfate", "Potrerillos", "Pimienta", "Cuyamel", "Pimienta", "La Unión", "Saba", "Santa Cruz de Yojoa", "Sultepeque", "Quimistán", "San Francisco de Yojoa", "San Manuel", "San Antonio de Cortés", "Santa Cruz de Yojoa", "La Lima", "Villanueva", "Choloma", "Omoa", "Puerto Cortés", "El Progreso", "Tela", "La Ceiba", "La Masica", "Tocoa", "Arizona", "Esparta", "Saba", "Santa Rosa de Aguán", "Santa Bárbara", "Iriona", "Balfate" } },
            { "Choluteca", new List<string> { "Choluteca", "San Marcos de Colón", "Namasigue", "Monjarás", "Apacilagua", "El Corpus", "Concepción de María", "Morolica", "San José", "San Isidro", "Orocuina", "Santa Ana de Yusguare", "Pespire", "San Antonio de Flores", "San José de Choluteca", "San Marcos de Colón", "Namasigue", "Monjarás", "Apacilagua", "El Corpus", "Concepción de María", "Morolica", "San José", "San Isidro", "Orocuina", "Santa Ana de Yusguare", "Pespire", "San Antonio de Flores" } },
            { "El Paraíso", new List<string> { "Yuscarán", "Danlí", "El Paraíso", "Guaimaca", "Texiguat", "Trojes", "Jacaleapa", "Teupasenti", "Ojojona", "Morocelí", "San Antonio", "San Ignacio", "San Juan de Flores", "San Lucas", "San Matías", "Santa Elena", "San Francisco de la Paz", "Sabanagrande", "San Antonio de Oriente", "San Juan de Flores", "San Lucas", "San Matías", "Santa Elena", "San Francisco de la Paz", "Sabanagrande", "San Antonio de Oriente" } },
            { "Francisco Morazán", new List<string> { "Tegucigalpa", "Comayagüela", "Tegucigalpa", "Distrito Central", "Tegucigalpa", "Comayagüela", "Distrito Central", "Tegucigalpa", "Comayagüela", "Alubaren", "Cedros", "Curarén", "El Rosario", "Guaimaca", "Jesús de Otoro", "La Paz", "Lepaterique", "Marale", "Nueva Armenia", "Ojojona", "Reitoca", "San Antonio de Oriente", "San Buenaventura", "San Ignacio", "San Juan de Flores", "San Miguelito", "Santa Ana", "Santa Catarina", "Santa Lucía", "Talanga", "Tatumbla", "Valle de Ángeles", "Villa de San Antonio", "Valle de Ángeles", "Villa de San Antonio" } },
            { "Gracias a Dios", new List<string> { "Puerto Lempira", "Brus Laguna", "Juan Francisco Bulnes", "Villeda Morales", "Barra Patuca", "Auás", "Mocorón", "Wampusirpe", "Tuyaboc", "Ahuya", "Kruta", "Jamastrán", "Boca del Río" } },
            { "Intibucá", new List<string> { "La Esperanza", "Intibucá", "Camasca", "Colomoncagua", "Concepción", "Dolores", "Guarita", "San Antonio", "San Isidro", "San Juan", "San Marco de Sierra", "San Miguelito", "Santa Lucía", "Yamaranguila", "Magdalena", "La Esperanza", "Intibucá", "Camasca", "Colomoncagua", "Concepción", "Dolores", "Guarita", "San Antonio", "San Isidro", "San Juan", "San Marco de Sierra", "San Miguelito", "Santa Lucía", "Yamaranguila", "Magdalena" } },
            { "Islas de la Bahía", new List<string> { "Roatán", "Utila", "Guanaja", "Barbareta", "Sant Elena", "Morat" } },
            { "La Paz", new List<string> { "La Paz", "Caballeros", "Cedeño", "Chinacla", "Guaimaca", "San Antonio", "San Juan", "San Pedro de Tutule", "San José", "La Paz", "Caballeros", "Cedeño", "Chinacla", "Guaimaca", "San Antonio", "San Juan", "San Pedro de Tutule", "San José", "La Paz", "Caballeros", "Cedeño", "Chinacla", "Guaimaca", "San Antonio", "San Juan", "San Pedro de Tutule", "San José" } },
            { "Lempira", new List<string> { "Gracias", "Belén Gualcho", "Candelaria", "Cololaca", "Erandique", "Guarita", "Gualcince", "La Campa", "La Iguala", "La Virtud", "Las Flores", "Lepaera", "Mapulaca", "Nueva Frontera", "Piraera", "San Andrés", "San Francisco", "San Juan", "San Manuel de Colohete", "San Rafael", "San Sebastián", "Santa Cruz", "Talgua", "Tambla", "Tomalá", "Virginia", "Gracias", "Belén Gualcho", "Candelaria", "Cololaca", "Erandique", "Guarita", "Gualcince", "La Campa", "La Iguala", "La Virtud", "Las Flores", "Lepaera", "Mapulaca", "Nueva Frontera", "Piraera", "San Andrés", "San Francisco", "San Juan", "San Manuel de Colohete", "San Rafael", "San Sebastián", "Santa Cruz", "Talgua", "Tambla", "Tomalá", "Virginia" } },
            { "Ocotepeque", new List<string> { "Ocotepeque", "Dolores Merendón", "Guarita", "La Encarnación", "La Labor", "Nueva Ocotepeque", "San Fernando", "San Francisco del Valle", "San Jorge", "Sensenti", "Sinuapa", "Ocotepeque", "Dolores Merendón", "Guarita", "La Encarnación", "La Labor", "Nueva Ocotepeque", "San Fernando", "San Francisco del Valle", "San Jorge", "Sensenti", "Sinuapa" } },
            { "Olancho", new List<string> { "Juticalpa", "Catacamas", "Campamento", "Concordia", "Dulce Nombre de Culmí", "El Rosario", "Esquipulas del Norte", "Guarita", "Guata", "Guayape", "Gualaco", "Jano", "Juticalpa", "La Unión", "Manto", "Meadela", "Mocorón", "Patuca", "Río Grande", "Salamá", "San Esteban", "San Francisco de Becerra", "San Francisco de la Paz", "San Jacinto", "San Lucas", "San Marcos", "Santa María del Real", "Silca", "Yocón", "Juticalpa", "Catacamas", "Campamento", "Concordia", "Dulce Nombre de Culmí", "El Rosario", "Esquipulas del Norte", "Guarita", "Guata", "Guayape", "Gualaco", "Jano", "Juticalpa", "La Unión", "Manto", "Meadela", "Mocorón", "Patuca", "Río Grande", "Salamá", "San Esteban", "San Francisco de Becerra", "San Francisco de la Paz", "San Jacinto", "San Lucas", "San Marcos", "Santa María del Real", "Silca", "Yocón" } },
            { "Santa Bárbara", new List<string> { "Santa Bárbara", "Arada", "Celaque", "Chinda", "Concepción", "Gualjoco", "Ilama", "Macuelizo", "Nueva Frontera", "Nueva Arada", "Petoa", "Quimistán", "San José de Colón", "San Luis", "San Marcos", "San Nicolás", "San Pedro", "San Vicente", "Santo Tomás", "Trinidad Copán", "Santa Bárbara", "Arada", "Celaque", "Chinda", "Concepción", "Gualjoco", "Ilama", "Macuelizo", "Nueva Frontera", "Nueva Arada", "Petoa", "Quimistán", "San José de Colón", "San Luis", "San Marcos", "San Nicolás", "San Pedro", "San Vicente", "Santo Tomás", "Trinidad Copán" } },
            { "Valle", new List<string> { "Nacaome", "Amapala", "Alianza", "Aramecina", "Caridad", "Goascorán", "Langue", "San Francisco de Coray", "San Lorenzo", "San Pedro", "Santa Ana", "Valle", "Nacaome", "Amapala", "Alianza", "Aramecina", "Caridad", "Goascorán", "Langue", "San Francisco de Coray", "San Lorenzo", "San Pedro", "Santa Ana", "Valle", "Nacaome", "Amapala", "Alianza", "Aramecina", "Caridad", "Goascorán", "Langue", "San Francisco de Coray", "San Lorenzo", "San Pedro", "Santa Ana" } },
            { "Yoro", new List<string> { "Yoro", "Arenal", "El Negrito", "El Progreso", "Jocón", "Morazán", "Plan de Flores", "Santa Rita", "Sulaco", "Victoria", "Yoro", "Yorito", "Arenal", "El Negrito", "El Progreso", "Jocón", "Morazán", "Plan de Flores", "Santa Rita", "Sulaco", "Victoria", "Yoro", "Yorito" } }
        };
    }

    public static List<string> GetDepartments()
    {
        return new List<string>(GetDepartmentsAndCities().Keys);
    }

    public static List<string> GetCitiesByDepartment(string department)
    {
        if (string.IsNullOrWhiteSpace(department))
            return new List<string>();

        var locations = GetDepartmentsAndCities();
        if (locations.TryGetValue(department, out var cities))
            return cities;

        return new List<string>();
    }
}
