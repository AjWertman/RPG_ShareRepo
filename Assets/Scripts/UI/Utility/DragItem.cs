using UnityEngine;
using UnityEngine.EventSystems;

namespace RPGProject.UI
{
    /// <summary>
    /// Allows a UI element to be dragged and dropped from and to a container.
    /// 
    /// Create a subclass for the type you want to be draggable. Then place on
    /// the UI element you want to make draggable.
    /// 
    /// During dragging, the item is reparented to the parent canvas.
    /// 
    /// After the item is dropped it will be automatically return to the
    /// original UI parent. It is the job of components implementing `IDragContainer`,
    /// `IDragDestination and `IDragSource` to update the interface after a drag
    /// has occurred.
    /// </summary>
    /// <typeparam name="T">The type that represents the item being dragged.</typeparam>
    public class DragItem<T> : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
        where T : class
    {
        Vector3 startPosition = Vector3.zero;
        Transform originalParent = null;
        IDragSource<T> source = null;

        Canvas parentCanvas = null;
        CanvasGroup canvasGroup = null;

        private void Awake()
        {
            parentCanvas = GetComponentInParent<Canvas>();
            source = GetComponentInParent<IDragSource<T>>();

            canvasGroup = GetComponent<CanvasGroup>();
        }

        private IDragDestination<T> GetContainer(PointerEventData _eventData)
        {
            if (_eventData.pointerEnter)
            {
                var container = _eventData.pointerEnter.GetComponentInParent<IDragDestination<T>>();

                return container;
            }
            return null;
        }

        private void DropItemIntoContainer(IDragDestination<T> _destination)
        {
            if (ReferenceEquals(_destination, source)) return;

            var destinationContainer = _destination as IDragContainer<T>;
            var sourceContainer = source as IDragContainer<T>;

            // Swap won't be possible
            if (destinationContainer == null || sourceContainer == null ||
                destinationContainer.GetItem() == null ||
                object.ReferenceEquals(destinationContainer.GetItem(), sourceContainer.GetItem()))
            {
                AttemptSimpleTransfer(_destination);
                return;
            }

            AttemptSwap(destinationContainer, sourceContainer);
        }

        private void AttemptSwap(IDragContainer<T> _destination, IDragContainer<T> _source)
        {
            // Provisionally remove item from both sides. 
            var removedSourceNumber = _source.GetNumber();
            var removedSourceItem = _source.GetItem();
            var removedDestinationNumber = _destination.GetNumber();
            var removedDestinationItem = _destination.GetItem();

            _source.RemoveItems(removedSourceNumber);
            _destination.RemoveItems(removedDestinationNumber);

            var sourceTakeBackNumber = CalculateTakeBack(removedSourceItem, removedSourceNumber, _source, _destination);
            var destinationTakeBackNumber = CalculateTakeBack(removedDestinationItem, removedDestinationNumber, _destination, _source);

            // Do take backs (if needed)
            if (sourceTakeBackNumber > 0)
            {
                _source.AddItems(removedSourceItem, sourceTakeBackNumber);
                removedSourceNumber -= sourceTakeBackNumber;
            }
            if (destinationTakeBackNumber > 0)
            {
                _destination.AddItems(removedDestinationItem, destinationTakeBackNumber);
                removedDestinationNumber -= destinationTakeBackNumber;
            }

            // Abort if we can't do a successful swap
            if (_source.MaxAcceptable(removedDestinationItem) < removedDestinationNumber ||
                _destination.MaxAcceptable(removedSourceItem) < removedSourceNumber ||
                removedSourceNumber == 0)
            {
                if (removedDestinationNumber > 0)
                {
                    _destination.AddItems(removedDestinationItem, removedDestinationNumber);
                }
                if (removedSourceNumber > 0)
                {
                    _source.AddItems(removedSourceItem, removedSourceNumber);
                }
                return;
            }

            // Do swaps
            if (removedDestinationNumber > 0)
            {
                _source.AddItems(removedDestinationItem, removedDestinationNumber);
            }
            if (removedSourceNumber > 0)
            {
                _destination.AddItems(removedSourceItem, removedSourceNumber);
            }
        }

        private bool AttemptSimpleTransfer(IDragDestination<T> _destination)
        {
            var draggingItem = source.GetItem();
            var draggingNumber = source.GetNumber();

            var acceptable = _destination.MaxAcceptable(draggingItem);
            var toTransfer = Mathf.Min(acceptable, draggingNumber);

            if (toTransfer > 0)
            {
                source.RemoveItems(toTransfer);
                _destination.AddItems(draggingItem, toTransfer);
                return false;
            }

            return true;
        }

        private int CalculateTakeBack(T _removedItem, int _removedNumber, IDragContainer<T> _removeSource, IDragContainer<T> _destination)
        {
            var takeBackNumber = 0;
            var destinationMaxAcceptable = _destination.MaxAcceptable(_removedItem);

            if (destinationMaxAcceptable < _removedNumber)
            {
                takeBackNumber = _removedNumber - destinationMaxAcceptable;

                var sourceTakeBackAcceptable = _removeSource.MaxAcceptable(_removedItem);

                // Abort and reset
                if (sourceTakeBackAcceptable < takeBackNumber)
                {
                    return 0;
                }
            }
            return takeBackNumber;
        }

        public void OnBeginDrag(PointerEventData _eventData)
        {
            startPosition = transform.position;
            originalParent = transform.parent;
            canvasGroup.blocksRaycasts = false;
            transform.SetParent(parentCanvas.transform, true);
        }

        public void OnDrag(PointerEventData _eventData)
        {
            transform.position = _eventData.position;
        }

        public void OnEndDrag(PointerEventData _eventData)
        {
            transform.position = startPosition;
            canvasGroup.blocksRaycasts = true;
            transform.SetParent(originalParent, true);

            IDragDestination<T> container;
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                container = parentCanvas.GetComponent<IDragDestination<T>>();
            }
            else
            {
                container = GetContainer(_eventData);
            }

            if (container != null)
            {
                DropItemIntoContainer(container);
            }
        }
    }
}