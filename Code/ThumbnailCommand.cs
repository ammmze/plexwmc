using System;

using Microsoft.MediaCenter.UI;


namespace PlexWMC
{
    /// <summary>
    /// An actionable item that also has an associated image.
    /// </summary>
    public class ThumbnailCommand : Command
    {
        /// <summary>
        /// Create an unowned ThumbnailCommand.
        /// </summary>
        public ThumbnailCommand()
            : base()
        {
        }

        /// <summary>
        /// Create a ThumbnailCommand that has an lifetime owner.
        /// </summary>
        public ThumbnailCommand(IModelItemOwner owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Title of this item.
        /// </summary>
        public string Title
        {
            get { return Description; }
            set
            {
                if (Description != value)
                {
                    Description = value;
                    FPC("Title");
                }
            }
        }

        /// <summary>
        /// The representative image for this command.
        /// </summary>
        public Image Image
        {
            get { return image; }
            set
            {
                if (image != value)
                {
                    image = value;
                    FPC("Image");
                }
            }
        }

        /// <summary>
        /// Some extra metadata to display about this item.
        /// </summary>
        public string Metadata
        {
            get { return metadata; }
            set
            {
                if (metadata != value)
                {
                    metadata = value;
                    FPC("Metadata");
                }
            }
        }

        /// <summary>
        /// Summary to display about this item.
        /// </summary>
        public string Summary
        {
            get { return summary; }
            set
            {
                if (summary != value)
                {
                    summary = value;
                    FPC("Summary");
                }
            }
        }

        //Our local version of FirePropertyChanged, optimised to make sure it only ever runs on the Application thread.
        internal void FPC(object Val)
        {

            String Value = (String)Val;

            if (Microsoft.MediaCenter.UI.Application.IsApplicationThread == true)

                base.FirePropertyChanged(Value);

            else
            {

                Microsoft.MediaCenter.UI.Application.DeferredInvoke(FPC, Value, new TimeSpan(1));

            }

        }


        private Image image = null;
        private string metadata = null;
        private string summary = null;
    }
}
