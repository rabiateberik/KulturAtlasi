namespace KulturAtlasi.Models
{
    public class RolAtaViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }

        // Hangi roller var ve hangileri seçili?
        public List<RolSecim> Roller { get; set; }
    }

    public class RolSecim
    {
        public string RolId { get; set; }
        public string RolAdi { get; set; }
        public bool SeciliMi { get; set; }
    }
}