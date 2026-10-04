using System.Collections.ObjectModel;

namespace CalculateurAge.ViewModels;

public class CalculateurViewModel : BaseViewModel
{
    private string _nom = "";
    private DateTime _dateNaissance = DateTime.Today.AddYears(-20);
    private string _resultat = "";
    private string _message = "";              // Majeur / Mineur
    private string _signe = "";                // Nouveau : signe astrologique
    private string _generation = "";           // Nouveau : génération
    private string _anniversaire = "";
    private bool _resultatVisible;

    public string Nom
    {
        get => _nom;
        set
        {
            if (SetField(ref _nom, value))
                CalculerCommand.Rafraichir();
        }
    }

    public DateTime DateNaissance
    {
        get => _dateNaissance;
        set => SetField(ref _dateNaissance, value);
    }

    public string Resultat
    {
        get => _resultat;
        set => SetField(ref _resultat, value);
    }

    public string Message
    {
        get => _message;
        set => SetField(ref _message, value);
    }

    public string Signe
    {
        get => _signe;
        set => SetField(ref _signe, value);
    }

    public string Generation
    {
        get => _generation;
        set => SetField(ref _generation, value);
    }

    public string Anniversaire
    {
        get => _anniversaire;
        set => SetField(ref _anniversaire, value);
    }

    public bool ResultatVisible
    {
        get => _resultatVisible;
        set => SetField(ref _resultatVisible, value);
    }

    public ObservableCollection<string> Historique { get; } = new();

    public RelayCommand CalculerCommand { get; }
    public RelayCommand EffacerCommand { get; }
    public RelayCommand ViderHistoriqueCommand { get; }

    public CalculateurViewModel()
    {
        CalculerCommand = new RelayCommand(
            Calculer,
            () => !string.IsNullOrWhiteSpace(Nom));

        EffacerCommand = new RelayCommand(Effacer);
        ViderHistoriqueCommand = new RelayCommand(() => Historique.Clear());
    }

    private void Calculer()
    {
        DateTime aujourdhui = DateTime.Today;

        if (DateNaissance.Date > aujourdhui)
        {
            Resultat = "La date de naissance est dans le futur";
            Message = "";
            Signe = "";
            Generation = "";
            Anniversaire = "";
            ResultatVisible = true;
            return;
        }

        int age = aujourdhui.Year - DateNaissance.Year;
        if (DateNaissance.Date > aujourdhui.AddYears(-age)) age--;

        Resultat = $"{Nom}, vous avez {age} ans";
        Message = age >= 18 ? "Majeur" : "Mineur";
        Signe = $"Signe : {CalculerSigne(DateNaissance)}";
        Generation = $"Génération : {CalculerGeneration(DateNaissance.Year)}";

        DateTime prochain = DateNaissance.Date.AddYears(age + 1);
        int jours = (prochain - aujourdhui).Days;
        Anniversaire = $"Prochain anniversaire dans {jours} jour(s)";

        Historique.Insert(0,
            $"{DateTime.Now:HH:mm} - {Nom} : {age} ans ({Message}), "
            + $"{CalculerSigne(DateNaissance)}, "
            + $"{CalculerGeneration(DateNaissance.Year)}");

        ResultatVisible = true;
    }

    private void Effacer()
    {
        Nom = "";
        DateNaissance = DateTime.Today.AddYears(-20);
        Resultat = "";
        Message = "";
        Signe = "";
        Generation = "";
        Anniversaire = "";
        ResultatVisible = false;
    }

    // Signe astrologique : on transforme la date en un entier MMJJ
    // (ex. 21 mars -> 321) pour comparer simplement aux bornes.
    private static string CalculerSigne(DateTime d)
    {
        int mmjj = d.Month * 100 + d.Day;
        return mmjj switch
        {
            >= 1222 or <= 119 => "Capricorne",
            <= 218 => "Verseau",
            <= 320 => "Poissons",
            <= 419 => "Bélier",
            <= 520 => "Taureau",
            <= 620 => "Gémeaux",
            <= 722 => "Cancer",
            <= 822 => "Lion",
            <= 922 => "Vierge",
            <= 1022 => "Balance",
            <= 1121 => "Scorpion",
            _ => "Sagittaire"
        };
    }

    // Génération d'après l'année de naissance.
    private static string CalculerGeneration(int annee) => annee switch
    {
        <= 1945 => "Génération silencieuse",
        <= 1964 => "Baby-boomer",
        <= 1980 => "Génération X",
        <= 1996 => "Millennial (Génération Y)",
        <= 2012 => "Génération Z",
        _ => "Génération Alpha"
    };
}