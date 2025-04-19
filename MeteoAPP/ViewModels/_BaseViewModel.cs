using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MeteoAPP.ViewModels
{
    /// <summary>
    /// Classe base astratta per tutti i ViewModel.
    /// Implementa <see cref="INotifyPropertyChanged"/> per supportare il binding dei dati.
    /// Include metodi ausiliari per il rilevamento delle modifiche alle proprietà.
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Evento notificato quando una proprietà cambia.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged; 

        /// <summary>
        /// Costruttore di base della classe ViewModel.
        /// </summary>
        protected BaseViewModel()
        {
        }

        /// <summary>
        /// Genera l'evento <see cref="PropertyChanged"/> per notificare al binding che una proprietà è cambiata.
        /// </summary>
        /// <param name="propertyName">Nome della proprietà che ha subito una modifica. Se non fornito, viene risolto automaticamente dal chiamante.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// Imposta il valore di una proprietà e notifica il cambiamento solo se il nuovo valore è diverso dal precedente.
        /// </summary>
        /// <typeparam name="T">Tipo della proprietà.</typeparam>
        /// <param name="backingStore">Riferimento al campo di backing della proprietà.</param>
        /// <param name="value">Nuovo valore da assegnare.</param>
        /// <param name="propertyName">Nome della proprietà che ha cambiato valore. Se non fornito, viene risolto automaticamente.</param>
        /// <returns>True se il valore è stato cambiato, false se era già uguale.</returns>
        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}