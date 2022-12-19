using UnityEngine;
using System.Collections;
using System;

namespace EasyMobile
{
    /// <summary>
    /// A struct representing the mutation of saved game information. Fields can either have a new value
    /// or be untouched (in which case the corresponding field in the saved game will be
    /// untouched). Instances must be built using <see cref="SavedGameInfoUpdate.Builder"/>.
    /// </summary>
    public struct SavedGameInfoUpdate
    {
        private readonly bool _descriptionUpdated;
        private readonly string _newDescription;
        private readonly bool _coverImageUpdated;
        private readonly byte[] _newPngCoverImage;
        private readonly bool _playedTimeUpdated;
        private readonly TimeSpan _newPlayedTime;

        private SavedGameInfoUpdate(Builder builder)
        {
            _descriptionUpdated = builder._descriptionUpdated;
            _newDescription = builder._newDescription;
            _coverImageUpdated = builder._coverImageUpdated;
            _newPngCoverImage = builder._newPngCoverImage;
            _playedTimeUpdated = builder._playedTimeUpdated;
            _newPlayedTime = builder._newPlayedTime;
        }

        public bool IsDescriptionUpdated
        {
            get
            {
                return _descriptionUpdated;
            }
        }

        public string UpdatedDescription
        {
            get
            {
                return _newDescription;
            }
        }

        public bool IsCoverImageUpdated
        {
            get
            {
                return _coverImageUpdated;
            }
        }

        public byte[] UpdatedPngCoverImage
        {
            get
            {
                return _newPngCoverImage;
            }
        }

        public bool IsPlayedTimeUpdated
        {
            get
            {
                return _playedTimeUpdated;
            }
        }

        public TimeSpan UpdatedPlayedTime
        {
            get
            {
                return _newPlayedTime;
            }
        }

        public struct Builder
        {
            internal bool _descriptionUpdated;
            internal string _newDescription;
            internal bool _coverImageUpdated;
            internal byte[] _newPngCoverImage;
            internal bool _playedTimeUpdated;
            internal TimeSpan _newPlayedTime;

            public Builder WithUpdatedDescription(string description)
            {
                _descriptionUpdated = true;
                _newDescription = description;
                return this;
            }

            public Builder WithUpdatedPngCoverImage(byte[] newPngCoverImage)
            {
                _coverImageUpdated = true;
                _newPngCoverImage = newPngCoverImage;
                return this;
            }

            public Builder WithUpdatedPlayedTime(TimeSpan newPlayedTime)
            {
                _playedTimeUpdated = true;
                _newPlayedTime = newPlayedTime;
                return this;
            }

            public SavedGameInfoUpdate Build()
            {
                return new SavedGameInfoUpdate(this);
            }
        }
    }
}

