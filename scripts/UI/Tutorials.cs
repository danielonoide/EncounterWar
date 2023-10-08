using Godot;
using System;

public class Tutorials : CanvasLayer
{
    Label title;
    ItemList itemList;
    TextureButton backButton, forwardButton;

    readonly string[][] items=new string[][]  //first is the title
    {
        new string[]
        {
            "Empezar partida",
            "Presiona el botón de empezar",
            "Empieza una nueva partida o \ncontinúa una       previamente guardada",
            "Selecciona el inventario del equipo acordado   con tu compañero",
            "Si pasas el ratón por una herramienta podrás   ver su descripción",
            "Una vez terminado, ambos presionen los       botones de Listo"
        },

        new string[]
        {
            "Inventario",
            "En tu turno, has clic en uno de los miembros de tu equipo para abrir el inventario",
            "La compra y venta funciona igual que en el   menú de selección de inventario",
            "Has clic en los íconos para seleccionar",
            "Usa la herramienta"
        },

        new string[]
        {
            "Herramientas arrojadizas",
            "Cuando veas un círculo debajo del integrante,  es que vas a usar una herramienta arrojadiza",
            "Arrastra el ratón para apuntar, y suelta para  lanzar",
            "No puedes apuntar directamente a otro         integrante"
        },

        new string[]
        {
            "Lanzaglobos",
            "Arrastra el ratón sobre la barra deslizadora   para apuntar, y suelta para lanzar uno de   sus globos, puedes lanzar 3 por turno",
            "No puedes apuntar directamente a otro        integrante"

        },

        new string[]
        {
            "Globo teledirigido",
            "Controla el globo mediante las teclas WASD o las de dirección",
            "También puedes mantener pulsado el ratón para mover el globo hacia esa dirección",
            "El globo no puede reventar directamente en un integrante, tienes que reventarlo en una     plataforma"
        },
    };

    VScrollBar scrollBar;
    byte currentIndex=0;


    public override void _Ready()
    {
        title=GetNode<Label>("CenterContainer/Title");
        itemList=GetNode<ItemList>("ItemList");
        backButton=GetNode<TextureButton>("BackButton");
        forwardButton=GetNode<TextureButton>("ForwardButton");

        backButton.Connect("pressed", this, nameof(_on_Button_pressed), new Godot.Collections.Array{true});
        forwardButton.Connect("pressed", this, nameof(_on_Button_pressed), new Godot.Collections.Array{false});

        SetItems();
        scrollBar= itemList.GetVScroll();
    }

    private void SetItems()
    {
        string[] strings=items[currentIndex];

        itemList.Clear();
        title.Text=strings[0];

        for(int i=1;i<strings.Length;i++)
        {
            var icon=GD.Load<Texture>($"res://sprites/Tutorials/{currentIndex}{i}.png");
            if(icon is not null)
            {
                itemList.AddItem(strings[i], GD.Load<Texture>($"res://sprites/Tutorials/{currentIndex}{i}.png"));
                continue;
            }

            itemList.AddItem(strings[i], GD.Load<Texture>($"res://sprites/Tutorials/{currentIndex}{i}.tres"));

        }

    }


    private void _on_Button_pressed(bool isBackButton)
    {
        int limit= isBackButton ? 0 : items.Length-1;

        if(currentIndex==limit)
        {
            return;
        }

        currentIndex= (byte)(isBackButton ? (byte)currentIndex-1 : (byte)currentIndex+1);
        SetItems();

        backButton.Disabled=currentIndex==0;
        forwardButton.Disabled=currentIndex==items.Length-1;

        scrollBar.Value=0;

    }

    private void _on_Close_pressed()
    {
        QueueFree();
    }

    private void _on_ItemList_gui_input(InputEvent @event)
    {
        if(@event is InputEventKey inputEventKey && inputEventKey.Pressed)
        {
            if(inputEventKey.Scancode ==(int)KeyList.Left)
            {
                _on_Button_pressed(true);
            }

            if(inputEventKey.Scancode ==(int)KeyList.Right)
            {
                _on_Button_pressed(false);
            }
        }
    }

    public static Tutorials GetTutorials()
    {
        PackedScene packedScene=GD.Load<PackedScene>("res://scenes/UI/Tutorials.tscn");
        Tutorials tutorials=packedScene.Instance<Tutorials>();

        return tutorials;
    }
}
