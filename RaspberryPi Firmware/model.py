import numpy as np
import pandas as pd

from sklearn import preprocessing
from sklearn.model_selection import train_test_split
from sklearn.tree import DecisionTreeClassifier
from sklearn.ensemble import RandomForestClassifier
from sklearn.svm import SVC
from sklearn.metrics import classification_report
from sklearn.externals import joblib

data = pd.read_csv("newdata.csv", engine = 'python')
data = data.sample(frac=1)

feature_names = ['A0', 'E0', 'A4', 'E4']
X = data[feature_names]

Y = data['COORD']
encoder = preprocessing.LabelEncoder()
encoder.fit(Y)
Y = encoder.transform(Y)

X_train, X_test, Y_train, Y_test = train_test_split(X, Y, test_size = 0.3)

svm = SVC(C=1.0, gamma=10.0, kernel='rbf')
svm.fit(X_train, Y_train)
true, pred = Y_test, svm.predict(X_test)
print(classification_report(true, pred))


joblib_file = "model.pkl"
joblib.dump(svm, joblib_file)

# sudo python3 model.py
import numpy as np
import pandas as pd

from sklearn import preprocessing
from sklearn.model_selection import train_test_split
from sklearn.tree import DecisionTreeClassifier
from sklearn.ensemble import RandomForestClassifier
from sklearn.svm import SVC
from sklearn.metrics import classification_report
from sklearn.externals import joblib

data = pd.read_csv("newdata.csv", engine = 'python')
data = data.sample(frac=1)

feature_names = ['A0', 'E0', 'A4', 'E4']
X = data[feature_names]

Y = data['COORD']
encoder = preprocessing.LabelEncoder()
encoder.fit(Y)
Y = encoder.transform(Y)

X_train, X_test, Y_train, Y_test = train_test_split(X, Y, test_size = 0.3)

svm = SVC(C=1.0, gamma=10.0, kernel='rbf')
svm.fit(X_train, Y_train)
true, pred = Y_test, svm.predict(X_test)
print(classification_report(true, pred))


joblib_file = "model.pkl"
joblib.dump(svm, joblib_file)

# sudo python3 model.py
