# Installation
Create conda environment with python version 3.9.13:
```sh
conda create -n ml-agents python=3.9.13
conda activate ml-agents
```

Follow next steps:
```sh
pip3 install torch~=1.13.1 -f https://download.pytorch.org/whl/torch_stable.html

pip install protobuf==3.20.3

pip install mlagents
```

If everything went fine, you should be able to run this command and see the list of commands:
```sh
mlagents-learn -h
```
