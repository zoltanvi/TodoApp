namespace Modules.Common.Views.DragDrop;

public interface IDropIndexModifier
{
    int GetModifiedDropIndex(int dropIndex, object droppedObject);
}
