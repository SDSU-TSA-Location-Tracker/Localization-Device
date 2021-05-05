import numpy as np
import pandas as pd

from sklearn import preprocessing
from sklearn.model_selection import train_test_split
from sklearn.tree import DecisionTreeClassifier
from sklearn.ensemble import RandomForestClassifier
from sklearn.ensemble import ExtraTreesClassifier
from sklearn.svm import SVC
from sklearn.metrics import classification_report
import joblib

data = pd.read_csv("newdata.csv", engine = 'python')
data = data.sample(frac=1)

feature_names = ['A0', 'E0', 'A4', 'E4']
X = data[feature_names]

Y = data['COORD']

encoder = preprocessing.LabelEncoder()
encoder.fit(Y)
Y = encoder.transform(Y)

X_train, X_test, Y_train, Y_test = train_test_split(X, Y, test_size = 0.3)

rfc2 = RandomForestClassifier(bootstrap=True,
                              criterion='gini',
                              max_depth=25,
                              max_features='log2',
                              min_samples_leaf=1,
                              min_samples_split=5, 
                              n_estimators=25)
rfc2.fit(X_train, Y_train)
print("RFC")
true, pred = Y_test, rfc2.predict(X_test)
print(classification_report(true, pred))


joblib_file = "rfc_small.pkl"
joblib.dump(rfc2, joblib_file)


svm = SVC(C=1.0, 
          gamma=10.0,
          kernel='rbf')
svm.fit(X_train, Y_train)
true, pred = Y_test, svm.predict(X_test)
print(classification_report(true, pred))

joblib_file = "svm_small.pkl"
joblib.dump(rfc2, joblib_file)




etc2 = ExtraTreesClassifier(bootstrap=True,
                            criterion='entropy',
                            max_depth=100,
                            max_features='log2',
                            min_samples_leaf=1,
                            min_samples_split=5,
                            n_estimators=50)
etc2.fit(X_train, Y_train)
true, pred = Y_test, etc2.predict(X_test)
print(classification_report(true, pred))

joblib_file = "etc_small.pkl"
joblib.dump(rfc2, joblib_file)


# sudo python3 model2.py
