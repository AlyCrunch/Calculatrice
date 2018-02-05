# Calculatrice

Comment j'ai procédé :

## 1. MVVM ##

### Rappel
Model–view–viewmodel c'est la façon dont tu structure ton code et donc comment les données transitent entre les couches
+ Model : Les données
+ ViewModel : Mise en forme des données
+ View : L'affichage des données

Dans notre exemple, on a pas besoin de la couche Model.

Habituellement on organise un projet comme ça :
+ App.xaml : Le premier fichier qui sera lu par ton application (tu peux y changer la vue lancée en premier)
+ **Controls** : 
+ **Helpers** : Toutes les classes qui peuvent être utilisée un peu partout et pas liée à une seule page
+ **Models** : Toutes les classes qui représentent des données
+ **ViewModels** : Tous tes ViewModels associés aux vues
  - MyViewModel.cs
  - **Dialogs** : Les ViewModels des popups
    * SelectItemDialogModel.cs
+ **Views** : Toutes tes vues
  - MyView.xaml
  - **Dialogs** : Toutes les pop up
    * SelectItemDialog.xaml

Après ça peut changer selon l'entreprise où tu te trouves mais généralement on est pas loin de ça.

Dans le dossier **Helpers** j'ai mis deux fichiers .cs qui sont deux classes utilent à l'architecture MVVM
Ces deux pages sont souvent copiées/collées d'internet et peuvent être ajoutées automatiquement si on utilise des Toolkit MVVM (MVVM Light par exemple)

### ViewModelBase.cs
Cette classe permet de "rafraichir" la vue quand la propriété est modifiée.
la méthode **SetProperty** est importante, on l'utilisera dans le ViewModel

### RelayCommand.cs
Cette classe permet de passer les informations d'action (le clic sur un bouton) plus facilement de la vue au viewmodel

## 2. Création du ViewModel ##

J'ai crée une classe dans le dossier ##ViewModel## que j'ai fait hériter de ViewModelBase.
### Getters / Setters
On sait que dans notre application, on aura en donnée : **La selection**, **L'historique** et moi j'ai décidé d'afficher le **resultat** final sur une autre valeur 
Donc j'ai créé 3 variables et leur accesseurs respectif :

``` cs
private string _current;
  public string Current
  {
      get { return _current; }
      set { SetProperty(ref _current, value); }
  }
```
J'ai utilisé le SetProperty de la classe mère **ViewModelBase**.
Pour chaque valeur qui sera utilisé sur la vue il faudra utiliser SetProperty.

### Commands
Je ne suis pas très habile des commands, mais c'est super utile. Il y a des moyen moins propres pour faire ça, je pourrais te montrer si tu veux.
Cette fois, tu réfléchis à toutes les actions que tu vas devoir faire et voir si tu peux les regrouper, j'ai identifié 4 groupes d'actions :
- toutes les actions sur chiffres (cliquer sur 1,2,3 ect)
- toutes les actions arthmétiques (cliquer sur +,-,/,*)
- l'action de corriger 
- l'action de valider

Dans le ViewModel une command se décompose en 3 "blocs" differents. Exemple avec l'action **valider**.

1. La variable bindable
Une variable RelayCommand (la classe ajoutée au début) qu'on va lier dans la vue
``` cs
private RelayCommand _equalCommand;
  public RelayCommand EqualCommand
  {
      get { return _equalCommand; }
      set { SetProperty(ref _equalCommand, value); }
  }
```

2. La méthode qui va s'executer à l'action
``` cs
public void HittingEqual()
  {
    //code ici
  }
```

3. Le lien entre la variable bindable et la méthode
Cette fois c'est une ligne qui se trouve dans la contructeur de la classe
``` cs
public MainViewModel()
  {
      EqualCommand = new RelayCommand(o => HittingEqual(), o => true);
      NumberCommand = new RelayCommand(o => HittingNumber(o.ToString()), o => true); //Pour passer un paramètre
  }
```

## 3. Création de la vue ##

J'ai ajouté un nouveau document de type "Fenêtre" ou "Window" au dossier **View**
Ton document est en deux parties : **TaVue.xaml** et **TaVue.xaml.cs**.
tous les elements purements graphiques se trouve dans le .xaml et le .cs sert à ajouter du code facilement, il faut essayer de **ne jamais y ajouter de code**, tout doit se faire dans ton ViewModel associé.

1. Lien avec le ViewModel
Une view fonctionne un peu comme une classe basique, pour utiliser la classe ViewModel tu dois lui ajouter un lien vers elle.
``` xaml
<Window xmlns:vm="clr-namespace:Calculatrice.ViewModel">
  <Window.DataContext>
      <vm:MainViewModel/>
  </Window.DataContext>
</Window>
```

2. Binding d'une variable
Une fois que le **DataContext** est bindé tu peux acceder aux variables du **ViewModel** de façon très simple en utilisant le mot clé **Binding**.
``` xaml
  <TextBlock x:Uid="CurrentTextBox" Text="{Binding Current}" />
```

3. Utilisation des commands
C'est aussi simple que le binding de Text, il suffit de binder la propriété **Command** d'un controle (un Button par exemple) et de passer (ou pas c'est selon le besoin) la valeur souhaitée à **CommandParameter**
``` xaml
<Button Command="{Binding NumberCommand}" CommandParameter="2" Content="2" />
```

4. Hotkeys
Les hotkeys sont définis dans une section spéciale du document et se définissent par 2 (ou 3) paramètres (La key, la command à executer et le paramètre optionnel)
``` xaml
<Window xmlns:vm="clr-namespace:Calculatrice.ViewModel">
    <Window.DataContext>
        <vm:MainViewModel/>
    </Window.DataContext>
    <Window.InputBindings>
        <KeyBinding Key="NumPad2" Command="{Binding NumberCommand}" CommandParameter="2"/>
    </Window.InputBindings>
</Window>
```

### Et c'est tout ###
Après ce n'est plus que de la logique et plus vraiment de la structure
