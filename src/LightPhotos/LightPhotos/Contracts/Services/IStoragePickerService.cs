﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;

namespace LightPhotos.Contracts.Services;
public interface IStoragePickerService
{
    IAsyncOperation<StorageFolder> PickSingleFolderAsync();
}
